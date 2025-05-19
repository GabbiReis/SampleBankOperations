# SampleBankOperations
**SampleBankOperations** é uma aplicação de console em C# que simula operações bancárias fundamentais, projetada com princípios de Clean Architecture para garantir modularidade e organização do código. 
O objetivo do projeto é servir como uma referência para a construção de sistemas modulares e escaláveis em C#, demonstrando operações bancárias como abertura de conta, consulta de saldo, depósitos, saques, transferências e cálculo de juros.

## Índice
* [Estrutura do Projeto](#estrutura-do-projeto)
* [Camadas e Principais Classes](camadas-e-principais-classes)
* [Operações Bancárias Implementadas](opercacoes-bancarias-implementadas)
* [Uso de Delegates](uso-de-delegates)
* [Execução](execucao)
* [Melhorias](melhorias)
* [Licença](licenca)

## Estrutura do Projeto
A estrutura de diretórios organiza o código em várias camadas para manter uma clara separação de responsabilidades.
SampleBankOperations
````
├── SampleBankOperations.Core
│   ├── Entities
│   │   └── Account.cs
│   ├── Enums
│   │   └── AccountType.cs
│   └── Interfaces
│       ├── IAccountRepository.cs
│       └── ILogger.cs
├── SampleBankOperations.Application
│   ├── Interfaces
│   │   └── IAccountService.cs
│   ├── Services
│   │   └── AccountService.cs
│   └── Validations
│       └── AccountValidator.cs
├── SampleBankOperations.Infrastructure
│   ├── Logging
│   │   └── ConsoleLogger.cs
│   ├── Persistence
│   │   ├── BankingContext.cs
│   │   └── Repositories
│   │       └── AccountRepository.cs
├── SampleBankOperations.App
│   ├── Services
│   │   ├── UI
│   │   │   └── UserInterface.cs
│   │   └── Operations
│   │       ├── IBankOperations.cs
│   │       └── BankOperations.cs
└── README.md
````
## Camadas e Principais Classes

### SampleBankOperations.Core: 
> Define as entidades principais, interfaces e enumerações do domínio bancário.

**Account.cs**: Representa uma conta bancária e encapsula métodos para operações básicas, como depósito e saque.
**AccountType.cs**: Enumeração que define tipos de conta (Checking e Savings).
**Interfaces**: Contém IAccountRepository para operações de persistência e ILogger para logging.

### SampleBankOperations.Application: 
> Camada de lógica de negócios que implementa operações bancárias, validações e serviços.

**AccountService.cs**: Implementa IAccountService e define as operações bancárias, como Deposit, Withdraw, Transfer, e CalculateInterest.
**AccountValidator.cs**: Contém métodos de validação (como saldo mínimo) utilizados nas operações bancárias.

### SampleBankOperations.Infrastructure: 
> Contém implementações de persistência e logging.

**BankingDbContext.cs**: Banco de dados em memória para armazenamento temporário das contas.
**AccountRepository.cs**: Implementação de IAccountRepository, fornecendo operações CRUD para contas.
**Logger.cs**: Implementação de ILogger que exibe logs no console.

### SampleBankOperations.App: 
> Contém a interface de usuário e a lógica de operações específicas.

**UserInterface.cs**: Classe responsável por interagir com o usuário, exibindo o menu e gerenciando a navegação entre operações.
**BankOperations.cs e IBankOperations.cs**: BankOperations encapsula a lógica de negócios das operações bancárias e implementa a interface IBankOperations.

## Operações Bancárias Implementadas

* **Abertura de Conta**: Cria uma nova conta com um saldo inicial especificado.
* **Consulta de Saldo**: Exibe o saldo atual da conta especificada.
* **Depósito**: Adiciona um valor ao saldo da conta e registra o depósito.
* **Saque**: Deduz um valor do saldo da conta, com verificação de saldo suficiente.
* **Transferência**: Transfere fundos de uma conta para outra, validando o saldo e registrando a operação.
* **Cálculo de Juros**: Calcula os juros com base na taxa fornecida e no saldo da conta.

## Uso de Delegates
O projeto utiliza `Func`, `Action` e `Predicate` para tornar as operações de conta mais flexíveis e modulares:

**Func** (_CalculateInterest_) - Calcula os juros ao aplicar uma função de cálculo sobre o saldo e a taxa de juros. Essa abordagem permite a troca da fórmula de cálculo sem alterar o código principal.
**Action** (_Deposit_) - Executa uma ação após um depósito, como registrar uma mensagem de confirmação ou logar a transação.
**Predicate** (_Withdraw_ e _Transfer_) - Valida uma condição antes de permitir a operação, como verificar se o saldo é suficiente para realizar o saque.

### Exemplo de Delegate em `AccountService`
```csharp
public bool Transfer(Account fromAccount, Account toAccount, decimal amount, Predicate<decimal> canWithdraw)
{
    if (!canWithdraw(fromAccount.Balance))
    {
        _logger.Log($"Insufficient balance in account {fromAccount.AccountNumber} for transfer of {amount:C}.");
        return false;
    }

    bool withdrawSuccess = Withdraw(fromAccount, amount, canWithdraw);
    if (!withdrawSuccess)
    {
        _logger.Log($"Failed to withdraw {amount:C} from account {fromAccount.AccountNumber}.");
        return false;
    }

    Deposit(toAccount, amount, amt => _logger.Log($"Deposited: {amt:C} to account {toAccount.AccountNumber}."));
    return true;
}

```
Essa abordagem torna a lógica de negócios mais flexível e facilita a substituição de regras e validações sem alterar o código principal.

## Execução
### Clonar o Repositório:
```
git clone https://github.com/usuario/SampleBankOperations.git
```

### Navegar até o Diretório do Projeto:
```
cd SampleBankOperations
```

### Executar o Projeto:
Compile e execute o projeto no Visual Studio ou utilize o comando:
```
dotnet run --project SampleBankOperations.App
```

### Interagir com o Menu:
Ao executar, um menu será exibido com as opções de operações bancárias. Selecione uma operação para realizar ações como depósito, saque ou consulta de saldo.

## Melhorias
Melhorias para implementação no projeto

- [ ] Inclusão de projeto de testes unitários
- [ ] Realizar _CodeCoverage_ de 80%
- [ ] Implementar tipo de conta
- [ ] Implementar design pattern _strategy_ para criar lógica de debito de valores em transferencias dependendo do tipo de conta
- [ ] Configurar persistência de dados utilizando EFCore
- [ ] Persistir dados em banco de dados postgres
- [ ] Implementar objeto de dados da conta
- [ ] Realizar configuração de chache redis de 1h para consulta de dados da conta (exceto salário)
- [ ] Configurar publicação de eventos para fila `sbo-account-created` via RabbitMq
- [ ] Configurar novo objeto para consulta de ações
- [ ] Configurar requisição via RestSharp em api pública para busca de valor de ação pelo ticker [Yahoo Finance API](https://developer.yahoo.com/sign-in-with-yahoo/?guccounter=1&guce_referrer=aHR0cHM6Ly9kZXZlbG9wZXIueWFob28uY29tL2FwaS8&guce_referrer_sig=AQAAAAwD6lwEWhW8bZ_nbASrr8hyPkGFSbyHoXkGbul8Snpf7mz5-I4S0x9yDhuLA-O1c_qrHA-Ii4w2EVrTKuU3hkxq9ktRv3y1HQ3pmhs1oLRDbOnKyPtAN3chIGRm_ALpY-cKCnal75xy3j9L4broTGOQTpAvOslpH9LSXw-bQcR2#overview)
- [ ] Orquestrar serviços de infra em arquivo docker-compose
- [ ] Conteinerizar aplicação `.dockerfile`
## Licença
Este projeto é licenciado sob a MIT License. Isso permite uso, modificação e distribuição, desde que a licença original seja incluída em qualquer redistribuição.
