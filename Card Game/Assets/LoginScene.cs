using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    public static LoginScene Instance;

    [SerializeField] private TextMeshProUGUI createAccountInfoText;
    [SerializeField] private InputField createEmailField;
    [SerializeField] private InputField createUsernameField;
    [SerializeField] private InputField createPasswordField;
    [SerializeField] private InputField createPasswordConfirmationField;
    [SerializeField] private Button createAccountButton;

    [SerializeField] private TextMeshProUGUI loginInfoText;
    [SerializeField] private InputField loginUsernameOrEmailField;
    [SerializeField] private InputField loginPasswordField;
    [SerializeField] private Button loginButton;

    [SerializeField] private Image UIBlocker;

    private void Start()
    {
        Instance = this;
    }

    public void submitCreateAccount()
    {
        string username = createUsernameField.text;
        string password1 = createPasswordField.text;
        string password2 = createPasswordConfirmationField.text;
        string email = createEmailField.text;

        // password validation
        if (password1 != password2)
        {
            Debug.Log("Password mismatch");
            if (createAccountInfoText != null)
                createAccountInfoText.text = "Passwords do not match";
            return;
        }
        if (password1.Length < 8)
        {
            Debug.Log("Password must be at least 8 characters");
            createAccountInfoText.text = "Password must be at least 8 characters";
            return;
        }
        if (password1.Length > 64)
        {
            Debug.Log("Maximum password length is 64 characters");
            createAccountInfoText.text = "Maximum password length is 64 characters";

            return;
        }

        // username validation
        if (username.Length < 4)
        {
            Debug.Log("Username must be at least 4 characters");
            createAccountInfoText.text = "Username must be at least 4 characters";

            return;
        }
        if (username.Length > 16)
        {
            Debug.Log("Username must be 16 characters or less");
            createAccountInfoText.text = "Username must be 16 characters or less";

            return;
        }
        if (!AccountUtils.IsUsername(username))
        {
            Debug.Log("Username contains invalid characters. Only use a-z (capital or lowercase) and 0-9");
            createAccountInfoText.text = "Username contains invalid characters. Only use a-z (capital or lowercase) and 0-9";

            return;
        }

        // email validation
        if (!AccountUtils.IsEmail(email))
        {
            Debug.Log(email + " is not an email");
            createAccountInfoText.text = email + " is not an email";

            return;
        }

        // all validation is done so can send server a message and lock the UI
        if (Client.Instance.getIsStarted()) // make sure we're connected to the internet first
        {
            lockUi();
            Net_CreateAccount netMsg = new Net_CreateAccount();
            netMsg.Email = email;
            netMsg.Password = password1;
            netMsg.Username = username;
            Client.Instance.SendServer(netMsg);
        }
        else
        {
            throw new Exception("Client is not connected to server");
        }
    }
    public void submitLoginRequest()
    {
        string usernameOrEmail = loginUsernameOrEmailField.text;
        string password = loginPasswordField.text;

        // validate username or email
        if (!AccountUtils.IsUsernameAndDiscriminator(usernameOrEmail) && !AccountUtils.IsEmail(usernameOrEmail))
        {
            loginInfoText.text = "Please user your email or username#0000 to sign in";
            loginInfoText.color = Color.red;
            return;
        }

        Net_LoginRequest lr = new Net_LoginRequest();
        lr.UsernameOrEmail = usernameOrEmail;
        lr.Password = password;
        Client.Instance.SendServer(lr);
    }
    private void lockUi()
    {
        UIBlocker.gameObject.SetActive(true);
        createAccountButton.interactable = false;
        loginButton.interactable = false;
    }

    private void unlockUI()
    {
        UIBlocker.gameObject.SetActive(false);
        createAccountButton.interactable = true;
        loginButton.interactable = true;
    }

    public void receiveLoginResponse(Net_LoginRequestResponse lrr)
    {

        Debug.Log("Server says " + lrr.information);
        Rect r = new Rect(1, 1, 100, 100);
        unlockUI();
        throw new NotImplementedException();
    }

    private Color green = new Color(.1f, 1f, .1f);
    private Color red = new Color(1f, .1f, .1f);
    internal void receiveCreateAccountResponse(Net_CreateAccountResponse msg)
    {
        switch (msg.success)
        {
            case CreateAccountResponseCode.success:
                createAccountInfoText.text = "Login successful";
                createAccountInfoText.color = Color.green;
                break;
            case CreateAccountResponseCode.emailAlreadyUsed:
                createAccountInfoText.text = "This email already has an associated account";
                createAccountInfoText.color = Color.red;
                break;
            case CreateAccountResponseCode.invalidEmail:
                createAccountInfoText.text = "Invalid email";
                createAccountInfoText.color = Color.red;
                break;
            case CreateAccountResponseCode.invalidUsername:
                createAccountInfoText.text = "Invalid username";
                createAccountInfoText.color = Color.red;
                break;
            case CreateAccountResponseCode.overUsedUsername:
                createAccountInfoText.text = "Popular username. Consider a different username";
                createAccountInfoText.color = Color.red;
                break;
        }
        unlockUI();
    }
}
