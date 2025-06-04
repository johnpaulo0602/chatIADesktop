#!/bin/bash
# Script para garantir que os modelos necessários estejam disponíveis no Ollama

# Configuração
OLLAMA_HOST=${OLLAMA_HOST:-"http://localhost:11434"}
DEFAULT_MODEL=${DEFAULT_MODEL:-"gemma3"}
TIMEOUT=${TIMEOUT:-300}

echo "Iniciando script para garantir disponibilidade dos modelos Ollama"
echo "Ollama Host: $OLLAMA_HOST"
echo "Modelo Padrão: $DEFAULT_MODEL"

# Função para verificar a disponibilidade do Ollama
check_ollama() {
    echo "Verificando se o serviço Ollama está disponível..."
    
    for i in {1..30}; do
        if curl -s "$OLLAMA_HOST/api/tags" > /dev/null; then
            echo "Serviço Ollama está disponível!"
            return 0
        fi
        
        echo "Aguardando serviço Ollama iniciar... ($i/30)"
        sleep 5
    done
    
    echo "Erro: Não foi possível conectar ao serviço Ollama após 30 tentativas."
    return 1
}

# Função para verificar se o modelo já existe
check_model() {
    local model=$1
    echo "Verificando se o modelo '$model' já está disponível..."
    
    # Buscar os modelos existentes
    local response=$(curl -s "$OLLAMA_HOST/api/tags")
    
    # Verificar se o modelo está na resposta
    if echo "$response" | grep -q "\"name\":\"$model\"" || echo "$response" | grep -q "\"name\":\"$model:latest\""; then
        echo "Modelo '$model' já está disponível!"
        return 0
    else
        echo "Modelo '$model' não encontrado."
        return 1
    fi
}

# Função para baixar o modelo
pull_model() {
    local model=$1
    echo "Baixando modelo '$model'..."
    
    curl -X POST "$OLLAMA_HOST/api/pull" -d "{\"name\":\"$model\"}" -H "Content-Type: application/json"
    
    if [ $? -eq 0 ]; then
        echo "Modelo '$model' baixado com sucesso!"
        return 0
    else
        echo "Erro ao baixar o modelo '$model'."
        return 1
    fi
}

# Execução principal
main() {
    # Verificar se o Ollama está disponível
    if ! check_ollama; then
        exit 1
    fi
    
    # Verificar e baixar o modelo padrão se necessário
    if ! check_model "$DEFAULT_MODEL"; then
        echo "Modelo não encontrado. Iniciando download..."
        if ! pull_model "$DEFAULT_MODEL"; then
            echo "Erro no download do modelo. Verifique o log do Ollama para mais detalhes."
            exit 1
        fi
    fi
    
    echo "Configuração de modelos Ollama concluída com sucesso!"
    exit 0
}

# Executar função principal
main