

# MainMotor API

**MainMotor** é uma API para plataforma online de revenda de veículos desenvolvida em C# e .NET 9 seguindo os princípios de Clean Architecture e SOLID. O sistema foca no cadastro, edição, venda e listagem de veículos, além de integração com sistemas de pagamento externos via webhook.

## 🚀 Funcionalidades Principais

- **Gestão de Veículos**: Cadastro, edição e listagem de veículos com filtros por status e ordenação por preço
- **Marketplace**: Listagem de veículos disponíveis e histórico de vendas
- **Registro de Vendas**: Sistema de vendas com validação de CPF e criação automática de clientes
- **Webhook de Pagamentos**: Esqueleto para integração com sistemas de pagamento externos
- **Documentação OpenAPI**: Interface Swagger completa para testes

## 🏗️ Arquitetura

O projeto segue **Clean Architecture** com separação clara de responsabilidades:

```
MainMotor/
├── MainMotor.Domain/          # Entidades de negócio e interfaces
├── MainMotor.Application/     # Casos de uso e serviços de aplicação
├── MainMotor.Infrastructure/  # Acesso a dados e serviços externos
└── MainMotor.API/            # Controllers e configuração da Web API
```

### Fluxo de Dependências
- **MainMotor.Domain**: Sem dependências externas
- **MainMotor.Application**: Referencia apenas Domain
- **MainMotor.Infrastructure**: Referencia Domain e Application
- **MainMotor.API**: Referencia todos os projetos (composition root)

## 🛠️ Stack Tecnológica

### Framework e Runtime
- **.NET 9**: Framework principal com performance otimizada
- **ASP.NET Core**: Web API framework com suporte nativo a JSON
- **C# 12**: Linguagem com recursos modernos

### Banco de Dados
- **PostgreSQL 15**: Banco de dados principal
- **Entity Framework Core 9**: ORM com suporte completo ao PostgreSQL
- **UUID v7**: Identificadores únicos ordenáveis por tempo

### Documentação
- **OpenAPI 3.0**: Documentação JSON da API
- **Swagger/Swashbuckle**: Geração automática da UI da documentação

### Containerização e Deploy
- **Docker**: Containerização da aplicação
- **Docker Compose**: Orquestração local
- **Kubernetes**: Deploy em produção com Helm
- **Helm**: Gerenciador de pacotes para Kubernetes
- **Multi-stage Dockerfile**: Build otimizado

### Principais Pacotes NuGet
```xml
<!-- Database -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0" />

<!-- API Documentation -->
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.8.1" />

<!-- Utilities -->
<PackageReference Include="UUIDNext" Version="1.0.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

## 🏛️ Decisões Arquiteturais

### Clean Architecture
A escolha da Clean Architecture foi baseada nos seguintes benefícios:

1. **Separação de Responsabilidades**: Cada camada tem uma responsabilidade específica
2. **Testabilidade**: Dependências invertidas facilitam testes unitários
3. **Manutenibilidade**: Mudanças em uma camada não afetam outras
4. **Flexibilidade**: Fácil troca de tecnologias (banco de dados, controllers, etc.)

### Estrutura de Camadas

#### Domain (Núcleo)
```
MainMotor.Domain/
├── Entities/          # Entidades de negócio
├── Enums/            # Enumerações do domínio
└── Repositories/     # Interfaces de repositório
```

#### Application (Casos de Uso)
```
MainMotor.Application/
├── DTOs/             # Data Transfer Objects
├── Interfaces/       # Contratos de serviços
├── Services/         # Implementação de casos de uso
├── Validators/       # Validações de negócio
└── Exceptions/       # Exceções customizadas
```

#### Infrastructure (Dados e Serviços Externos)
```
MainMotor.Infrastructure/
├── Data/             # DbContext e configurações EF
├── Repositories/     # Implementações de repositório
├── Services/         # Serviços
└── Migrations/       # Migrações do banco
```

#### API (Interface)
```
MainMotor.API/
├── Controllers/     # Endpoints da API
├── Models/          # ViewModels e DTOs da API
├── Filters/         # Filtros customizados
└── Middleware/      # Middlewares customizados
```

#### Diagrama do banco de dados
![Diagrama do banco de dados](https://raw.githubusercontent.com/Zaniquelli/MainMotor/refs/heads/main/docs/database.png)

### Padrões Implementados

#### Repository Pattern
- Abstração do acesso a dados
- Facilita testes unitários
- Permite troca de tecnologia de persistência

#### Dependency Injection
- Inversão de controle nativa do .NET
- Configuração centralizada no `Program.cs`
- Facilita testes e manutenção

#### DTO Pattern
- Separação entre entidades de domínio e dados de transporte
- Controle sobre dados expostos na API
- Versionamento de API facilitado

### Escolhas Técnicas

#### PostgreSQL vs SQL Server
**Escolhido: PostgreSQL**
- Open source e gratuito
- Excelente performance
- Suporte nativo a UUID
- Compatibilidade com containers

#### UUID v7 vs GUID/Auto-increment
**Escolhido: UUID v7**
- Ordenação temporal natural
- Distribuição melhor em índices
- Compatibilidade com sistemas distribuídos
- Segurança (não sequencial)

#### Entity Framework vs Dapper
**Escolhido: Entity Framework Core**
- Migrations automáticas
- Change tracking
- LINQ integrado
- Produtividade de desenvolvimento

#### Swagger vs Outras Documentações
**Escolhido: Swagger/OpenAPI**
- Padrão da indústria
- Interface interativa
- Geração automática
- Integração com ferramentas

## 🚀 Configuração e Execução

### Pré-requisitos
- .NET 9 SDK
- Docker e Docker Compose
- Minikube: Para executar clusters Kubernetes localmente
- Kubectl: Para gerenciar os recursos do cluster Kubernetes
- Helm: Para gerenciar os pacotes Kubernetes
- PostgreSQL

### 1. Execução com Docker (Recomendado)

```bash
# Clone o repositório
git clone https://github.com/Zaniquelli/MainMotor.git
cd MainMotor

# Execute com Docker Compose (inclui PostgreSQL)
docker-compose up -d

# A API/Swagger UI estará disponível em http://localhost:8080
```

### 2. Deploy em Kubernetes com Minikube

#### Script Automatizado para Windows
```bash
# Execute o script que configura tudo automaticamente
./deploy-minikube.bat

# Verificar status dos pods
kubectl get pods

# Acessar a API
minikube service mainmotor-api --url
```

#### Método Manual
```bash
# Iniciar o Minikube
minikube start

# Configurar Docker para usar o daemon do Minikube
# Windows PowerShell:
& minikube -p minikube docker-env --shell powershell | Invoke-Expression

# Windows CMD:
@FOR /f "tokens=*" %i IN ('minikube -p minikube docker-env --shell cmd') DO @%i

# Linux/Mac:
eval $(minikube docker-env)

# Construir a imagem no contexto do Minikube
docker build -t mainmotor-api:latest .

# Deploy com Helm
helm upgrade --install mainmotor-api ./helm

# Verificar status
kubectl get pods
kubectl get services

# Acessar a API
minikube service mainmotor-api --url
```

#### Comandos Úteis para Minikube
```bash
# Ver logs da aplicação
kubectl logs -f deployment/mainmotor-api

# Acessar o dashboard do Kubernetes
minikube dashboard

# Parar o Minikube
minikube stop

# Deletar o cluster
minikube delete
```

#### Comandos Úteis do Helm
```bash
# Upgrade da aplicação
helm upgrade mainmotor ./helm

# Rollback para versão anterior
helm rollback mainmotor 1

# Status do release
helm status mainmotor

# Desinstalar
helm uninstall mainmotor
```

## 📊 Banco de Dados

### Dados de Referência (Seed Data)

O sistema inclui dados completos de:
- **Marcas e Modelos**: Dados brasileiros com códigos FIPE
- **Anos**: Configurações por modelo
- **Características**: Tipos e valores para classificação de veículos

## ⚙️ Helm Chart

O projeto inclui um Helm Chart simplificado para deploy no Kubernetes:

### Estrutura do Chart
```
helm/
├── Chart.yaml              # Metadados do chart
├── values.yaml             # Configurações padrão
└── templates/
    ├── configmap.yaml      # Configurações da API
    ├── secret.yaml         # Connection string
    ├── deployment.yaml     # Deploy da API
    ├── service.yaml        # Service da API
    ├── postgres-local.yaml # PostgreSQL para desenvolvimento
    └── hpa.yaml           # Auto-scaling
```

### Configuração (values.yaml)
```yaml
app:
  name: mainmotor-api
  image: mainmotor-api:latest
  imagePullPolicy: Never  # Importante para Minikube
  port: 8080
  replicas: 2

postgres:
  enabled: true
  database: mainmotor
  username: postgres
  password: postgres
  port: 5432
```

**Nota**: O `imagePullPolicy: Never` é essencial para o Minikube, pois força o Kubernetes a usar apenas imagens locais, evitando o erro `ImagePullBackOff`.

### Comandos Essenciais
```bash
# Install/Upgrade (recomendado)
helm upgrade --install mainmotor-api ./helm

# Uninstall
helm uninstall mainmotor-api

# Ver valores computados
helm get values mainmotor-api

# Listar releases
helm list

# Ver status do release
helm status mainmotor-api
```

## 🐛 Tratamento de Erros

A API retorna códigos HTTP apropriados:

- `200 OK` - Operação bem-sucedida
- `201 Created` - Recurso criado
- `400 Bad Request` - Dados inválidos
- `404 Not Found` - Recurso não encontrado
- `409 Conflict` - Conflito de estado (ex: veículo já vendido)
- `500 Internal Server Error` - Erro interno

### Exemplo de Resposta de Erro
```json
{
  "title": "Validation Error",
  "status": 400,
  "errors": {
    "CustomerCpf": ["CPF deve ter 11 dígitos"]
  }
}
```

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 🚀 Kubernetes Production Setup

### Deploy com Helm
```bash
# Deploy em produção (customizar values.yaml conforme necessário)
helm install mainmotor ./helm --namespace mainmotor --create-namespace

# Verificar status
kubectl get pods -n mainmotor
kubectl get services -n mainmotor

# Verificar release do Helm
helm list -n mainmotor
```

### Monitoramento
```bash
# Logs da aplicação
kubectl logs -f deployment/mainmotor-api -n mainmotor

# Métricas de recursos
kubectl top pods -n mainmotor
```

## 🔧 Troubleshooting

### Problemas Comuns

#### API não inicia (Docker)
```bash
# Verificar logs
docker-compose logs mainmotor-api

# Verificar conexão com banco
docker-compose exec postgres psql -U postgres -d mainmotor -c "SELECT 1;"
```

#### ImagePullBackOff no Minikube
```bash
# Verificar se a imagem existe no daemon do Minikube
eval $(minikube docker-env)  # Linux/Mac
# ou
& minikube -p minikube docker-env --shell powershell | Invoke-Expression  # Windows

docker images | grep mainmotor

# Se não existir, construir novamente
docker build -t mainmotor-api:latest .

# Verificar se imagePullPolicy está como Never
kubectl describe pod <pod-name> | grep "Image Pull Policy"
```

#### Conflitos de Release no Helm
```bash
# Limpar releases conflitantes
helm uninstall <release-name>

# Limpar recursos órfãos
kubectl delete secret mainmotor-api-secret
kubectl delete configmap mainmotor-api-config

# Reinstalar
helm upgrade --install mainmotor-api ./helm
```

---

Victor Zaniquelli.