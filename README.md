# Sistema de Controle de Gastos Residenciais

Aplicação full stack para controle de gastos residenciais, com cadastro de pessoas, cadastro de
transações (receitas/despesas) e consulta de totais.

- **Back-end:** .NET 8 (ASP.NET Core Web API) + Entity Framework Core + PostgreSQL
- **Front-end:** React + TypeScript (Vite)
- **Persistência:** PostgreSQL, via Docker Compose

## Como rodar

```bash
# Back-end + banco de dados
docker-compose up --build

# Front-end (em outro terminal)
cd frontend
npm install
npm run dev
```

- Back-end (Swagger): `http://localhost:5000/swagger`
- Front-end: `http://localhost:5173`

Não é necessário nenhum passo manual de banco de dados ao subir, a API aplica as migrations
automaticamente no PostgreSQL.

## Arquitetura

| Camada         | Tecnologia                                   |
|----------------|------------------------------------------------|
| Back-end       | .NET 8 / ASP.NET Core Web API                  |
| Arquitetura    | Controller → Service → DbContext (EF Core)     |
| Banco de dados | PostgreSQL 16 (via Docker)                     |
| Front-end      | React 19 + TypeScript (Vite)                   |
| HTTP client    | axios                                           |
| Orquestração   | Docker Compose (banco + API)                    |

A lógica de negócio fica na camada de **Service** (`Services/`), não nos Controllers. Os
Controllers só traduzem HTTP em chamadas ao Service e devolvem a resposta. Regras de negócio
violadas (ex: pessoa não encontrada, menor de idade tentando cadastrar receita) são sinalizadas
lançando exceptions de domínio (`NaoEncontradoException`, `RegraDeNegocioException`), capturadas
por um middleware global (`ExceptionHandlingMiddleware`) que as traduz em respostas HTTP (404,
400) — evitando `try/catch` repetido em cada endpoint.

## Principais decisões técnicas

- **PostgreSQL via Docker**: o enunciado exige persistência após fechar a aplicação. Um banco
  relacional rodando como serviço independente garante isso; PostgreSQL foi escolhido por ser
  robusto, amplamente usado no mercado, e por familiaridade.
- **Ids como Guid, gerados pelo banco** (`gen_random_uuid()`): evita colisão em cenários
  distribuídos e não expõe informação sequencial, ao contrário de um ID numérico incremental.
- **Valor da transação sempre positivo**: quem define entrada/saída de dinheiro é o campo `Tipo`
  (Receita/Despesa), não o sinal do número evita ambiguidade no cálculo de saldo.
- **Cascade delete configurado no banco** (`OnDelete(DeleteBehavior.Cascade)`), não em código:
  ao excluir uma Pessoa, o próprio PostgreSQL remove as Transações vinculadas, sem necessidade de
  lógica manual no Service.
- **Enum `TipoTransacao` serializado como texto** (`"Receita"`/`"Despesa"`) tanto no banco quanto
  na API, em vez do padrão numérico do C# deixa o contrato da API e os dados no banco mais
  legíveis.
- **Migrations do EF Core aplicadas automaticamente** (`db.Database.Migrate()` no `Program.cs`):
  a API sobe já com o schema atualizado, sem passo manual.
- **Container da API roda como usuário não-root**: reduz o impacto de uma eventual
  vulnerabilidade explorada dentro do container.
- **DTOs separados das entidades**: os Models (`Pessoa`, `Transacao`) nunca são expostos
  diretamente pela API. DTOs de entrada (`CriarPessoaDto`, `CriarTransacaoDto`) não aceitam um Id
  vindo do cliente (quem gera é o banco), e DTOs de saída controlam exatamente o que é exposto.

## Regras de negócio implementadas

**Pessoa**
- Id gerado automaticamente (Guid), Nome (obrigatório) e Idade (obrigatória, 0–150).
- Ao deletar uma pessoa, todas as suas transações são deletadas em cascata.

**Transação**
- Id gerado automaticamente (Guid), Descrição, Valor (> 0), Tipo (Receita/Despesa) e PessoaId.
- O `PessoaId` informado precisa existir no cadastro de pessoas — caso contrário, `400 Bad
  Request`.
- Se a pessoa for menor de idade (idade < 18), somente transações do tipo **Despesa** podem ser
  cadastradas para ela; tentar cadastrar Receita retorna `400 Bad Request`.
- Não há edição nem deleção de transações não exigido pelo enunciado.

**Totais**
- Para cada pessoa: soma de receitas, soma de despesas e saldo (`receitas - despesas`).
- Ao final: total geral de receitas, total geral de despesas e saldo líquido geral.

## Endpoints da API

| Método | Rota                | Descrição                                                    |
|--------|----------------------|----------------------------------------------------------------|
| GET    | `/api/pessoas`       | Lista todas as pessoas                                          |
| GET    | `/api/pessoas/{id}`  | Busca uma pessoa por Id                                         |
| POST   | `/api/pessoas`       | Cria uma pessoa (`{ nome, idade }`)                             |
| DELETE | `/api/pessoas/{id}`  | Remove uma pessoa (e suas transações, em cascata)               |
| GET    | `/api/transacoes`    | Lista todas as transações (aceita `?pessoaId=`)                 |
| POST   | `/api/transacoes`    | Cria uma transação (`{ descricao, valor, tipo, pessoaId }`)     |
| GET    | `/api/totais`        | Totais de receitas/despesas/saldo por pessoa + total geral      |

## Possíveis evoluções

O enunciado permite recursos adicionais desde que não afetem o funcionamento do que já foi
especificado. Fora do escopo por tempo, mas seriam evoluções naturais:
- Edição e deleção de transações.
- Testes automatizados (unitários para os Services, integração para os endpoints).
- Autenticação/autorização (hoje a API é aberta).
- Paginação e filtros mais avançados na listagem de transações.