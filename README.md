# Chat IA Desktop

Uma aplicação Blazor Server que permite conversar com modelos de IA usando o Ollama localmente no seu computador.

## Funcionalidades

- 💬 Chat com modelos de IA local via Ollama
- 📁 Upload e análise de arquivos PDF e TXT
- 🔍 Comandos especiais (@ajuda.ai e @sentimento)
- 🌙 Tema escuro/claro
- 🔒 Processamento local (privacidade)

## Pré-requisitos

- .NET 8.0 SDK
- Ollama instalado e em execução (via Docker ou localmente)

## Configuração e Execução

### 1. Iniciar o Ollama

```bash
cd ollama
docker-compose up -d
```

### 2. Executar a aplicação

```bash
cd ChatIADesktop
dotnet run
```

Acesse a aplicação em [http://localhost:5000](http://localhost:5000) ou [https://localhost:5001](https://localhost:5001)

## Comandos Especiais

- `@ajuda.ai [termo]` - Explica um termo em linguagem simples (máx. 100 palavras)
- `@sentimento [texto]` - Analisa o sentimento de um texto (positivo/negativo/neutro)

## Estrutura do Projeto

- **Components/Pages**: Interfaces Blazor
- **Servicos**: Lógica de negócios e comunicação com Ollama
- **Modelos**: Classes de dados
- **Utilitarios**: Ferramentas auxiliares

## Tecnologias

- .NET 8.0
- Blazor Server
- Bootstrap 5
- iText7 (para processamento de PDF)
- Ollama (API de IA local)