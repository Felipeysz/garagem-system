# Modelagem do banco — GAR

## Visão geral

```
Oficina (1) ──< Veiculo >── (1) Cliente
   │                │
   │                └──< OrdemServico >── (1) Funcionario
   │                         │
   │                         ├──(1:1)── Orcamento
   │                         └──< OrdemServicoServico >── (1) Servico >── (1) TipoServico
   │
   ├──< Funcionario
   ├──< Fornecedor
   ├──< TipoServico
   ├──< Servico
   ├──< Peca >──< MovimentacaoEstoque >── Fornecedor (opcional) / OrdemServico (opcional)
   └──< OrdemServico
```

`Oficina` é a raiz do sistema: toda outra entidade, com exceção de `Cliente`, se relaciona a ela direta ou indiretamente através de uma chave estrangeira `OficinaId`.

## Tipos de Id e chave estrangeira

Todo `Id` de tabela é `BIGINT IDENTITY`. As colunas de chave estrangeira (`OficinaId`, `ClienteId`, `VeiculoId` etc.) também são `BIGINT`, porque referenciam diretamente esses `Id`s — o tipo da FK segue sempre o tipo da PK que ela referencia.

## Cliente não pertence a uma Oficina

`Cliente` não tem `OficinaId`. Essa relação foi retirada porque um mesmo cliente pode levar seu(s) veículo(s) a oficinas diferentes — a associação entre um cliente e uma oficina não é fixa, ela muda conforme o veículo. Por isso o vínculo com `OficinaId` fica em `Veiculo`, não em `Cliente`: é o `Veiculo` que pertence a uma oficina específica, e o `Cliente` se conecta a oficinas indiretamente, através dos veículos que tem cadastrados em cada uma.

Como consequência dessa relação, `Cliente.CPF` é único em todo o banco (`UNIQUE` global) — não existe um "CPF por oficina" já que o Cliente não está preso a nenhuma oficina especificamente.

## Unicidade por oficina

Nas entidades que pertencem a uma oficina (`Funcionario`, `Fornecedor`, `TipoServico`, `Servico`, `Veiculo`, `Peca`), a unicidade de identificadores reais (CPF, CNPJ, Placa, Chassi, Nome, CodigoInterno) é aplicada dentro do escopo de cada oficina (`UNIQUE (OficinaId, campo)`), não globalmente. Isso permite, por exemplo, que duas oficinas diferentes tenham cada uma seu próprio funcionário com o mesmo cargo/nome de tipo de serviço, sem colisão entre elas.

`Chassi` (em `Veiculo`) e `CodigoInterno` (em `Peca`) são campos opcionais no domínio; a unicidade correspondente (`UQ_Veiculo_OficinaChassi`, `UQ_Peca_OficinaCodigoInterno`) é aplicada só quando o campo está preenchido (`WHERE ... IS NOT NULL`), permitindo múltiplos registros com o campo em branco.

## Relação 1:1 entre OrdemServico e Orcamento

`Orcamento.OrdemServicoId` tem constraint `UNIQUE`, não só uma FK simples — cada `OrdemServico` tem no máximo um `Orcamento` associado.

## OrdemServicoServico

Tabela associativa entre `OrdemServico` e `Servico` (relação N:N), com o campo adicional `Observacoes` por vínculo. A constraint `UQ_OSS_OrdemServicoId_ServicoId` impede que o mesmo serviço apareça duas vezes na mesma ordem de serviço.

## Status como texto livre

`OrdemServico.Status` e `Orcamento.Status` são `NVARCHAR(50)`, sem `CHECK` nem enum no banco. Essa representação corresponde à forma como esses campos estão declarados atualmente no Domain (`string`).

## MovimentacaoEstoque.Tipo

Armazenado como texto (`'Entrada'` / `'Saida'`) com `CHECK`, correspondendo ao enum `TipoMovimentacaoEstoque` do Domain — a conversão de enum para texto reflete diretamente os dois valores existentes nesse enum.

## Colunas de auditoria/controle

`DataCadastro`, `DataCriacao`, `DataAbertura` e `DataMovimentacao` têm `DEFAULT SYSUTCDATETIME()`, preenchendo automaticamente a data/hora UTC no momento da inserção quando o valor não é informado explicitamente pela aplicação. Campos `Ativo` (em `Oficina`, `Cliente`, `Veiculo`, `Funcionario`, `Fornecedor`, `TipoServico`, `Servico`, `Peca`) têm `DEFAULT 1`, refletindo o valor padrão `true` que essas propriedades já têm no C#.
