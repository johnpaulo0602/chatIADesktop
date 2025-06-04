# Chat IA Desktop

Uma aplicação Blazor Server que permite conversar com modelos de IA usando o Ollama localmente no seu computador.

## Funcionalidades

- 💬 Chat com modelos de IA local via Ollama
- 📁 Upload e análise de arquivos PDF e TXT
- 🔍 Comandos especiais (@ajuda.ai e @sentimento)
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
   
4. **IMPORTANTE**: O modelo de IA será baixado automaticamente pelo container ollama-init, mas você pode verificar o status com:
```bash
docker logs ollama-init
```

### Modelos Ollama

O container ollama-init foi configurado para baixar automaticamente o modelo padrão (gemma3). Se você quiser usar modelos alternativos:

1. Baixe manualmente outros modelos conforme necessário:
```bash
docker exec -it ollama ollama pull gemma3:12b
# ou
docker exec -it ollama ollama pull llama2
```

2. Altere o modelo no chat usando a interface do usuário, ou modifique o modelo padrão em:
   - `appsettings.Production.json` para alteração permanente
   - Variável de ambiente `Ollama__ModeloPadrao` no docker-compose.yaml

3. Verifique os modelos disponíveis com:
```bash
docker exec -it ollama ollama list
```

### Solucionando Erros 404 da API Ollama

Se você encontrar erros 404 ao tentar utilizar a aplicação, provavelmente o modelo não está disponível no Ollama. Para resolver:

1. Verifique os logs do Ollama:
```bash
docker logs ollama
```

2. Certifique-se de que o modelo configurado em appsettings.json (gemma3) foi baixado:
```bash
docker exec -it ollama ollama list
```

3. Se necessário, baixe o modelo manualmente:
```bash
docker exec -it ollama ollama pull gemma3:latest
```

### Configurando HTTPS para Docker (Opcional)

Por padrão, a aplicação usa apenas HTTP para simplificar o desenvolvimento local. Se você precisar de HTTPS:

1. Gere um certificado SSL para desenvolvimento:
```bash
dotnet dev-certs https -ep cert.pfx -p SuaSenhaAqui
```

2. Atualize o docker-compose.yaml:
```yaml
blazor:
  # Outras configurações...
  volumes:
    - ./cert.pfx:/app/cert.pfx
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - ASPNETCORE_URLS=http://+:80;https://+:443
    - ASPNETCORE_Kestrel__Certificates__Default__Path=/app/cert.pfx
    - ASPNETCORE_Kestrel__Certificates__Default__Password=SuaSenhaAqui
  ports:
    - "5000:80"
    - "5001:443"
```

3. Reinicie os containers:
```bash
docker-compose down && docker-compose up -d
```

Para mais informações sobre HTTPS no Docker, consulte a [documentação oficial](https://learn.microsoft.com/pt-br/aspnet/core/security/docker-https).

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

## Tecnologias Utilizadas

- .NET 8.0
- Blazor Server
- Tailwind CSS
- iText7 (para processamento de PDF)
- Ollama (API de IA local)