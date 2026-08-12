# GRA — Roadmap Application & Infra

## Status atual

- ✅ **Domain** completo: 12 entidades mapeadas a partir do banco `GAR` (`GRA.Domain/Entities`) + enum `TipoMovimentacaoEstoque`.
- ✅ **Infra.Persistence** com EF Core InMemory configurado (`GRAContext` + `AddInfraPersistence()`), sem repositórios ainda.
- ⏳ **Application**: vazio, é o próximo passo.
- ⏳ **Infra.Persistence (repositórios)**: depende da Application estar pronta (as interfaces nascem lá).

Fluxo de dependência: `Application` define as interfaces (contratos) → `Infra.Persistence` implementa essas interfaces usando o `GRAContext`.

---

## Antes de dividir: alinhar os dois juntos

Pra evitar conflito de merge e estilo divergente, o ideal é Felipe e Danilo decidirem **juntos, antes de começar**, e um dos dois já deixar criado:

1. **`GRA.Application/Common/Interfaces/IRepository.cs`** — repositório genérico base:
   ```csharp
   public interface IRepository<T> where T : class
   {
       Task<T?> GetByIdAsync(int id);
       Task<IEnumerable<T>> GetAllAsync();
       Task AddAsync(T entity);
       void Update(T entity);
       void Remove(T entity);
       Task<int> SaveChangesAsync();
   }
   ```
2. **Padrão de DTO**: sufixo `Request` (entrada) e `Response` (saída), records em vez de classes.
3. **Padrão de retorno dos Services**: usar exceptions de domínio simples (ex: `NotFoundException`, `BusinessRuleException`) ou um `Result<T>` — decidir um só padrão pros dois.
4. **Estrutura de pastas**: uma pasta por agregado/entidade, sempre com o mesmo miolo (`I{Entidade}Repository.cs`, `I{Entidade}Service.cs`, `{Entidade}Service.cs`, `DTOs/`).

Sugestão: **Felipe** cria esse esqueleto comum (`Common/Interfaces`, exceptions, DTO base) já que `Garagem` (dele) é a raiz do tenant, e sobe isso antes de todo mundo começar a codar em paralelo.

---

## Estrutura de pastas proposta

```
GRA.Application/
  Common/
    Interfaces/
      IRepository.cs
    Exceptions/
      NotFoundException.cs
      BusinessRuleException.cs
  Garagens/
    IGaragemRepository.cs
    IGaragemService.cs
    GaragemService.cs
    DTOs/
      GaragemRequest.cs
      GaragemResponse.cs
  Clientes/
    ...
  (uma pasta por entidade, mesmo padrão)

GRA.Infra.Persistence/
  Context/
    GRAContext.cs            (já existe)
  Repositories/
    GaragemRepository.cs
    ClienteRepository.cs
    ...
```

---

## Divisão de tarefas

Split em dois blocos de 6 entidades cada, agrupadas por proximidade de domínio.

### 🟦 Felipe — Cadastros Base

| Entidade | Observações |
|---|---|
| `Garagem` | Raiz do tenant. Cria também o esqueleto comum (ver seção acima). |
| `Cliente` | CPF único por garagem. |
| `Veiculo` | Placa e Chassi únicos por garagem; validar que o `Cliente` pertence à mesma `Garagem`. |
| `Funcionario` | CPF único por garagem. |
| `TipoServico` | Nome único por garagem. |
| `Servico` | Nome único por garagem; validar que `TipoServico` pertence à mesma `Garagem`. |

### 🟩 Danilo — Operacional (Estoque & Ordem de Serviço)

| Entidade | Observações |
|---|---|
| `Fornecedor` | CNPJ único por garagem. |
| `Peca` | Nome único por garagem. |
| `OrdemServico` | Fluxo mais complexo: abertura, finalização, status, vínculo com `Veiculo`/`Funcionario`. |
| `Orcamento` | Relação 1:1 com `OrdemServico` — não pode haver dois orçamentos pra mesma OS. |
| `OrdemServicoServico` | Tabela associativa OS x Serviço. |
| `MovimentacaoEstoque` | Regra de negócio mais delicada: saída não pode deixar estoque negativo (calculado a partir do somatório de entradas/saídas, já que a tabela não guarda saldo atual). |

---

## Passo a passo por entidade (repetir pra cada uma da sua lista)

1. **`I{Entidade}Repository.cs`** (Application) — estende `IRepository<T>` e adiciona métodos específicos de consulta (ex: `GetByCpfAsync`, `ExisteVeiculoComPlacaAsync`).
2. **DTOs** (Application) — `{Entidade}Request` e `{Entidade}Response`.
3. **`I{Entidade}Service.cs` + `{Entidade}Service.cs`** (Application) — regras de negócio e orquestração (validações, uso do repositório).
4. *(Depois, quando o time avançar pra Infra)* **`{Entidade}Repository.cs`** (Infra.Persistence) — implementação concreta usando `GRAContext` (o InMemory já está configurado, é só implementar as queries).
5. Registrar o Service e o Repository no DI — coordenar essa parte pra não haver conflito no `Program.cs`/`DependencyInjection.cs` (idealmente cada um mexe só na sua leva de linhas, ou um finaliza a lista dos dois no final).

---

## Regras de negócio pra ter em mente (não exaustivo)

- **Cliente / Funcionario / Fornecedor**: CPF/CNPJ únicos *por garagem*, não globalmente.
- **Veiculo**: Placa e Chassi únicos por garagem; `ClienteId` precisa pertencer à mesma `GaragemId` da `Veiculo`.
- **Servico**: `TipoServicoId` precisa pertencer à mesma `GaragemId`.
- **OrdemServico**: `VeiculoId` e `FuncionarioResponsavelId` precisam pertencer à mesma `GaragemId`; não finalizar sem passar por regras mínimas (ex: pelo menos 1 serviço vinculado — a definir com o time).
- **Orcamento**: um por `OrdemServico` (unicidade já garantida no banco/EF, mas validar na Application antes de tentar salvar, pra dar erro de negócio claro).
- **MovimentacaoEstoque**: `Tipo = Saida` não pode deixar estoque calculado negativo; `FornecedorId` só faz sentido em `Entrada`.

---

## Convenção de Git sugerida

- Branches: `feature/felipe-application-cadastros` e `feature/danilo-application-operacional`.
- PRs pequenos, um por entidade (ou par de entidades relacionadas, ex: `Orcamento` + `OrdemServicoServico`), pra facilitar review e reduzir conflito.
- Quem terminar primeiro pode ajudar revisando o PR do outro antes de começar a etapa de Infra.

## Ordem geral sugerida

1. Alinhar e subir o esqueleto comum (`IRepository<T>`, exceptions, padrão de DTO).
2. Cada um implementa a Application do seu bloco (interfaces, DTOs, Services).
3. Merge e revisão cruzada.
4. Implementar os repositórios concretos na Infra.Persistence (cada um faz o seu bloco, igual à Application).
5. Testes manuais via Swagger/endpoints simples na `GRA.Api` (ainda sem controllers reais além do WeatherForecast de exemplo — criar conforme for necessário).
