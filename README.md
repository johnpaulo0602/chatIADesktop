# Chat IA Desktop

Uma aplicação Blazor Server que permite conversar com modelos de IA usando o Ollama localmente no seu computador.

## Funcionalidades

- 💬 Chat com modelos de IA local via Ollama
- 📁 Upload e análise de arquivos PDF e TXT
- 🔍 Comandos especiais (@ajuda.ai e @sentimento)
- 🌙 Tema escuro moderno
- 🔒 Processamento local (privacidade)

## Clonando o Repositório

Para obter o código-fonte, clone o repositório:

```bash
git clone https://github.com/johnpaulo0602/chatIADesktop.git
cd chatIADesktop
```

## Executando localmente para Desenvolvimento

### Instalando Dependências no Windows

1. Instale o .NET 8.0 SDK:
   - Baixe o instalador do [site oficial da Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Execute o instalador e siga as instruções na tela
   - Verifique a instalação:
   ```cmd
   dotnet --version
   ```

2. Instale o Ollama:
   - Baixe o instalador do [site oficial do Ollama](https://ollama.ai/download/windows)
   - Execute o instalador e siga as instruções
   - Abra o PowerShell e execute o Ollama:
   ```powershell
   ollama serve
   ```

3. Baixe um modelo de IA (em outro terminal):
   ```powershell
   ollama pull gemma3:latest
   ```

### Instalando Dependências no Linux (Fedora/Ubuntu)

1. Instale o .NET 8.0 SDK no Ubuntu/Debian:
   ```bash
   wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   rm packages-microsoft-prod.deb
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-8.0
   ```

   No Fedora:
   ```bash
   sudo dnf install dotnet-sdk-8.0
   ```

2. Instale o Ollama no Linux:
   ```bash
   curl -fsSL https://ollama.com/install.sh | sh
   ollama serve &
   ```

3. Baixe um modelo de IA:
   ```bash
   ollama pull gemma3:latest
   ```

### Executando a Aplicação Localmente

1. Restaure as dependências:
   ```bash
   dotnet restore
   ```

2. Compile a aplicação:
   ```bash
   dotnet build
   ```

3. Execute a aplicação:
   ```bash
   dotnet run
   ```

4. Acesse a aplicação em:
   - http://localhost:5000
   - https://localhost:5001

## Executando com Docker (Alternativa)

### Pré-requisitos
- Docker
- Docker Compose
- Placa de vídeo NVIDIA com drivers instalados (opcional, para aceleração GPU)

### Passos para Execução

1. Construa a imagem Docker da aplicação Blazor:
```bash
docker build -t chat-blazor .
```

2. Inicie os containers com Docker Compose:
```bash
docker-compose up -d
```

3. A aplicação estará disponível em:
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001

4. Baixe o modelo de IA necessário:
```bash
docker exec -it ollama ollama pull gemma3:latest
```

Para baixar modelos alternativos, como o gemma3:12b:
```bash
docker exec -it ollama ollama pull gemma3:12b
```

### Parando a Aplicação Docker

```bash
docker-compose down
```

Para remover volumes e dados:
```bash
docker-compose down -v
```

## Comandos Especiais

- `@ajuda.ai [termo]` - Explica um termo em linguagem simples (máx. 100 palavras)
- `@sentimento [texto]` - Analisa o sentimento de um texto (positivo/negativo/neutro)

## Modelos Recomendados

- **gemma3:latest**: Modelo padrão, bom equilíbrio entre desempenho e qualidade
- **gemma3:12b**: Versão maior com melhor qualidade de resposta
- **llama2**: Alternativa para hardware menos potente
- **mistral**: Bom para tarefas de geração de texto

## Estrutura do Projeto

- **Components/Pages**: Interfaces Blazor
- **Servicos**: Lógica de negócios e comunicação com Ollama
- **Modelos**: Classes de dados
- **Utilitarios**: Ferramentas auxiliares

## Resolução de Problemas

- **Certificado HTTPS não confiável**: Adicione uma exceção de segurança no navegador
- **Modelo não carregado**: Verifique os logs: `docker logs ollama` ou `ollama logs`
- **Erro de conexão**: Verifique se o serviço Ollama está rodando
- **Performance lenta**: Modelos menores como o gemma3:2b ou mistral:7b exigem menos recursos
- **Porta em uso**: Altere as portas no docker-compose.yaml ou na execução local

## Tecnologias Utilizadas

- .NET 8.0
- Blazor Server
- Tailwind CSS
- iText7 (para processamento de PDF)
- Ollama (API de IA local)