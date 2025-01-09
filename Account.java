class Account {
    private String accountId;
    private String ownerName;
    private double balance;

    public Account(String accountId, String ownerName, double balance) {
        this.accountId = accountId;
        this.ownerName = ownerName;
        this.balance = balance;
    }

    public String GetAccountId() {
        return accountId;
    }

    public String GetOwnerName() {
        return ownerName;
    }

    public double GetBalance() {
        return balance;
    }

    public void Deposit(double amount) {
        if (amount > 0) {
            balance += amount;
        }
    }

    public boolean Withdraw(double amount) {
        if (amount > 0 && amount <= balance) {
            balance -= amount;
            return true;
        }
        return false;
    }

    public boolean Transfer(Account toAccount, double amount) {
        if (this.Withdraw(amount)) {
            toAccount.Deposit(amount);
            return true;
        }
        return false;
    }
}
