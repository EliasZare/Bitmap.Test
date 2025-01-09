import java.util.ArrayList;
import java.util.List;

class User {
    private String username;
    private String password;
    private List<Account> accounts;

    public User(String username, String password) {
        this.username = username;
        this.password = password;
        this.accounts = new ArrayList<>();
    }

    public String GetUsername() {
        return username;
    }

    public String GetPassword() {
        return password;
    }

    public List<Account> GetAccounts() {
        return accounts;
    }

    public void AddAccount(Account account) {
        accounts.add(account);
    }
}
