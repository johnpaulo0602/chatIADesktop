# Explicação Servicos

Este documento explica o propósito e o funcionamento de cada serviço.

## Visão Geral

A pasta `Servicos` contém classes de serviço que encapsulam a lógica de negócios da aplicação. O projeto utiliza o padrão de injeção de dependência com interfaces que definem contratos e implementações concretas. Todos os serviços são projetados para interagir com o Ollama, um serviço de IA que processa linguagem natural.

## Serviços Implementados

### 1. ServicoOllama (IServicoOllama)

**Propósito**: Responsável pela comunicação com a API Ollama, que fornece os recursos de IA para a aplicação.

**Funcionalidades principais**:
- Processamento de mensagens via API Ollama
- Gerenciamento dos modelos disponíveis
- Troca de modelo em uso
- Listagem de modelos disponíveis

**Como funciona**:
- Utiliza HttpClient para fazer requisições à API REST do Ollama
- Carrega configurações como URL base, modelo padrão e timeout de um arquivo de configuração
- Implementa tratamento de erros para falhas de conexão e timeout
- Serializa/deserializa respostas JSON da API

### 2. ProcessadorArquivos (IProcessadorArquivos)

**Propósito**: Extrai texto de diferentes tipos de arquivos e gera resumos.

**Funcionalidades principais**:
- Processamento de arquivos PDF e TXT
- Extração de texto dos arquivos
- Geração de resumos para textos longos
- Verificação de formatos de arquivo suportados

**Como funciona**:
- Utiliza a biblioteca iText para processar arquivos PDF
- Lê arquivos TXT diretamente com StreamReader
- Utiliza o ServicoOllama para gerar resumos de textos longos
- Implementa um mecanismo de fallback para truncar textos quando a geração de resumo falha

### 3. AnalisadorSentimento (IAnalisadorSentimento)

**Propósito**: Analisa o sentimento de textos usando o Ollama.

**Funcionalidades principais**:
- Análise de sentimento de textos
- Classificação em "Positivo", "Negativo" ou "Neutro"

**Como funciona**:
- Envia um prompt estruturado para o Ollama solicitando análise de sentimento
- Normaliza a resposta para garantir que esteja dentro das categorias esperadas
- Retorna um objeto `ResultadoAnalise` com o texto original e o resultado da análise

### 4. ExplicadorTermos (IExplicadorTermos)

**Propósito**: Fornece explicações simples para termos técnicos.

**Funcionalidades principais**:
- Explicação de termos técnicos em linguagem simples

**Como funciona**:
- Envia um prompt ao Ollama solicitando uma explicação simplificada
- Limita a explicação a aproximadamente 100 palavras
- Retorna um objeto `ResultadoAnalise` com o termo original e sua explicação

### 5. ServicoExemplos (IServicoExemplos)

**Propósito**: Gera exemplos de perguntas para a interface do usuário.

**Funcionalidades principais**:
- Geração de perguntas de exemplo para exibir na interface
- Cache de perguntas para evitar chamadas repetidas à API

**Como funciona**:
- Utiliza o Ollama para gerar perguntas relevantes e interessantes
- Implementa um mecanismo de cache para armazenar perguntas por 1 hora
- Filtra as respostas para assegurar que sejam perguntas válidas
- Fornece perguntas padrão em caso de falha na geração

## Padrões e Princípios

1. **Injeção de Dependência**: Todos os serviços utilizam injeção de dependência para facilitar testes e desacoplamento.

2. **Princípio da Responsabilidade Única**: Cada serviço tem uma responsabilidade bem definida.

3. **Tratamento de Erros**: Implementação consistente de tratamento de exceções e fallbacks.

4. **Assincronicidade**: Uso de métodos assíncronos (Task) para operações de I/O e comunicação com APIs.

5. **Cache**: Utilização de cache para otimizar operações repetitivas.

## Relacionamentos entre Serviços

- Todos os serviços (exceto ServicoOllama) dependem de IServicoOllama para processamento de linguagem natural.
- Os serviços são independentes entre si, interagindo apenas através do código cliente (provavelmente controladores ou viewmodels).

## Configuração

Os serviços são configurados através de:
- Injeção de dependência (configurada no startup da aplicação)
- Arquivos de configuração para parâmetros como URL base, timeout e modelo padrão