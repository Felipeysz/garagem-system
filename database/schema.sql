-- ============================================================================
-- GRA — Schema alinhado com GRA.Domain (branch feature/ajustes-iniciais)
-- Gerado a partir das entidades em src/GRA.Domain/Entities.
-- Ids e FKs em BIGINT, acompanhando Entity.Id (long).
-- Script completo: cria o banco se não existir, remove as tabelas se já
-- existirem e recria tudo do zero. Rode o backup antes se o banco GAR já
-- tiver dados que precisam ser preservados.
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'GAR')
BEGIN
    CREATE DATABASE GAR;
END
GO

USE GAR;
GO

-- Remove as tabelas na ordem inversa das dependências (filhas antes das mães)
DROP TABLE IF EXISTS MovimentacaoEstoque;
DROP TABLE IF EXISTS Orcamento;
DROP TABLE IF EXISTS OrdemServicoServico;
DROP TABLE IF EXISTS OrdemServico;
DROP TABLE IF EXISTS Peca;
DROP TABLE IF EXISTS Servico;
DROP TABLE IF EXISTS TipoServico;
DROP TABLE IF EXISTS Fornecedor;
DROP TABLE IF EXISTS Funcionario;
DROP TABLE IF EXISTS Veiculo;
DROP TABLE IF EXISTS Cliente;
DROP TABLE IF EXISTS Oficina;
GO

CREATE TABLE Oficina (
    Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
    Nome          NVARCHAR(200)  NOT NULL,
    CNPJ          NVARCHAR(14)   NOT NULL,
    Telefone      NVARCHAR(20)   NULL,
    Email         NVARCHAR(200)  NULL,
    Endereco      NVARCHAR(300)  NULL,
    DataCadastro  DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    Ativo         BIT            NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Oficina_CNPJ UNIQUE (CNPJ)
);

CREATE TABLE Cliente (
    Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
    Nome          NVARCHAR(200)  NOT NULL,
    CPF           NVARCHAR(11)   NOT NULL,
    Telefone      NVARCHAR(20)   NULL,
    Email         NVARCHAR(200)  NULL,
    DataCadastro  DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    Ativo         BIT            NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Cliente_CPF UNIQUE (CPF)
);

CREATE TABLE Veiculo (
    Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId       BIGINT         NOT NULL,
    ClienteId       BIGINT         NOT NULL,
    Placa           NVARCHAR(8)    NOT NULL,
    Chassi          NVARCHAR(17)   NULL,
    Marca           NVARCHAR(100)  NOT NULL,
    Modelo          NVARCHAR(100)  NOT NULL,
    Ano             INT            NOT NULL,
    Cor             NVARCHAR(30)   NULL,
    Quilometragem   INT            NOT NULL,
    Observacoes     NVARCHAR(500)  NULL,
    Ativo           BIT            NOT NULL DEFAULT 1,
    CONSTRAINT FK_Veiculo_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id),
    CONSTRAINT FK_Veiculo_Cliente FOREIGN KEY (ClienteId) REFERENCES Cliente(Id),
    CONSTRAINT UQ_Veiculo_OficinaPlaca UNIQUE (OficinaId, Placa)
);
CREATE UNIQUE INDEX UQ_Veiculo_OficinaChassi ON Veiculo(OficinaId, Chassi) WHERE Chassi IS NOT NULL;

CREATE TABLE Funcionario (
    Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId     BIGINT         NOT NULL,
    Nome          NVARCHAR(200)  NOT NULL,
    CPF           NVARCHAR(11)   NOT NULL,
    Telefone      NVARCHAR(20)   NULL,
    Email         NVARCHAR(200)  NULL,
    Cargo         NVARCHAR(100)  NOT NULL,
    DataAdmissao  DATE           NOT NULL,
    Ativo         BIT            NOT NULL DEFAULT 1,
    CONSTRAINT FK_Funcionario_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id),
    CONSTRAINT UQ_Funcionario_OficinaCPF UNIQUE (OficinaId, CPF)
);

CREATE TABLE Fornecedor (
    Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId     BIGINT         NOT NULL,
    RazaoSocial   NVARCHAR(200)  NOT NULL,
    NomeFantasia  NVARCHAR(200)  NULL,
    CNPJ          NVARCHAR(14)   NOT NULL,
    Telefone      NVARCHAR(20)   NULL,
    Email         NVARCHAR(200)  NULL,
    Endereco      NVARCHAR(300)  NULL,
    DataCadastro  DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    Ativo         BIT            NOT NULL DEFAULT 1,
    CONSTRAINT FK_Fornecedor_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id),
    CONSTRAINT UQ_Fornecedor_OficinaCNPJ UNIQUE (OficinaId, CNPJ)
);

CREATE TABLE TipoServico (
    Id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId   BIGINT         NOT NULL,
    Nome        NVARCHAR(150)  NOT NULL,
    Descricao   NVARCHAR(500)  NULL,
    Ativo       BIT            NOT NULL DEFAULT 1,
    CONSTRAINT FK_TipoServico_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id),
    CONSTRAINT UQ_TipoServico_OficinaNome UNIQUE (OficinaId, Nome)
);

CREATE TABLE Servico (
    Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId       BIGINT         NOT NULL,
    TipoServicoId   BIGINT         NOT NULL,
    Nome            NVARCHAR(150)  NOT NULL,
    Descricao       NVARCHAR(500)  NULL,
    TempoEstimado   INT            NULL,
    Ativo           BIT            NOT NULL DEFAULT 1,
    CONSTRAINT FK_Servico_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id),
    CONSTRAINT FK_Servico_TipoServico FOREIGN KEY (TipoServicoId) REFERENCES TipoServico(Id),
    CONSTRAINT UQ_Servico_OficinaNome UNIQUE (OficinaId, Nome)
);

CREATE TABLE Peca (
    Id             BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId      BIGINT         NOT NULL,
    Nome           NVARCHAR(200)  NOT NULL,
    Descricao      NVARCHAR(500)  NULL,
    CodigoInterno  NVARCHAR(50)   NULL,
    UnidadeMedida  NVARCHAR(10)   NULL,
    PrecoVenda     DECIMAL(10,2)  NULL,
    EstoqueMinimo  INT            NOT NULL,
    Ativo          BIT            NOT NULL DEFAULT 1,
    CONSTRAINT FK_Peca_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id)
);
CREATE UNIQUE INDEX UQ_Peca_OficinaCodigoInterno ON Peca(OficinaId, CodigoInterno) WHERE CodigoInterno IS NOT NULL;

CREATE TABLE OrdemServico (
    Id                        BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId                 BIGINT         NOT NULL,
    VeiculoId                 BIGINT         NOT NULL,
    FuncionarioResponsavelId  BIGINT         NOT NULL,
    DataAbertura              DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    DataFinalizacao           DATETIME2      NULL,
    QuilometragemEntrada      INT            NOT NULL,
    Status                    NVARCHAR(50)   NOT NULL,
    Observacoes               NVARCHAR(500)  NULL,
    CONSTRAINT FK_OrdemServico_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id),
    CONSTRAINT FK_OrdemServico_Veiculo FOREIGN KEY (VeiculoId) REFERENCES Veiculo(Id),
    CONSTRAINT FK_OrdemServico_Funcionario FOREIGN KEY (FuncionarioResponsavelId) REFERENCES Funcionario(Id)
);

CREATE TABLE OrdemServicoServico (
    Id             BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId      BIGINT         NOT NULL,
    OrdemServicoId BIGINT         NOT NULL,
    ServicoId      BIGINT         NOT NULL,
    Observacoes    NVARCHAR(500)  NULL,
    CONSTRAINT FK_OSS_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id),
    CONSTRAINT FK_OSS_OrdemServico FOREIGN KEY (OrdemServicoId) REFERENCES OrdemServico(Id),
    CONSTRAINT FK_OSS_Servico FOREIGN KEY (ServicoId) REFERENCES Servico(Id),
    CONSTRAINT UQ_OSS_OrdemServicoId_ServicoId UNIQUE (OrdemServicoId, ServicoId)
);

CREATE TABLE Orcamento (
    Id             BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId      BIGINT         NOT NULL,
    OrdemServicoId BIGINT         NOT NULL,
    DataCriacao    DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    DataAprovacao  DATETIME2      NULL,
    Status         NVARCHAR(50)   NOT NULL,
    Observacoes    NVARCHAR(500)  NULL,
    CONSTRAINT FK_Orcamento_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id),
    CONSTRAINT FK_Orcamento_OrdemServico FOREIGN KEY (OrdemServicoId) REFERENCES OrdemServico(Id),
    CONSTRAINT UQ_Orcamento_OrdemServicoId UNIQUE (OrdemServicoId)
);

CREATE TABLE MovimentacaoEstoque (
    Id               BIGINT IDENTITY(1,1) PRIMARY KEY,
    OficinaId        BIGINT         NOT NULL,
    PecaId           BIGINT         NOT NULL,
    FornecedorId     BIGINT         NULL,
    OrdemServicoId   BIGINT         NULL,
    Tipo             NVARCHAR(20)   NOT NULL,
    Quantidade       INT            NOT NULL,
    PrecoUnitario    DECIMAL(10,2)  NULL,
    DataMovimentacao DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    Observacoes      NVARCHAR(500)  NULL,
    CONSTRAINT FK_Movimentacao_Oficina FOREIGN KEY (OficinaId) REFERENCES Oficina(Id),
    CONSTRAINT FK_Movimentacao_Peca FOREIGN KEY (PecaId) REFERENCES Peca(Id),
    CONSTRAINT FK_Movimentacao_Fornecedor FOREIGN KEY (FornecedorId) REFERENCES Fornecedor(Id),
    CONSTRAINT FK_Movimentacao_OrdemServico FOREIGN KEY (OrdemServicoId) REFERENCES OrdemServico(Id),
    CONSTRAINT CK_Movimentacao_Tipo CHECK (Tipo IN ('Entrada', 'Saida'))
);
GO
