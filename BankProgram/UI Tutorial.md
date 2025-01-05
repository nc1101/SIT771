# Tutorial - Bank Program UI with SplashKit

## Background
This brief tutorial seeks to demonstrate how SplashKit can be used to implement a simple UI for the `BankProgram` being built out in SIT771. The codebase used as the basis for the tutorial included all relevant tasks up to (and including) '5.3 Many Accounts'.

## Objectives
This tutorial will demonstrate:
- a basic approach to using SplashKit's Interface elements and functions
- a practical example of a form for the 'Transfer' operation
- how `BankProgram` methods can be adapted to act on UI inputs

## Tutorial

> All changes discussed in this section are to `Program.cs`.

### 1. Create a window
The first step in introducing a UI to the BankProgram application is to create a window in which to render the interface (as demonstrated in other programs throughout the trimester).
```C#
Window window = new Window("Bank Program", 800, 600);
```

### 2. Create fields to accommodate form inputs
As the tutorial is focused on the 'Transfer' operation, three fields are required to accommodate user inputs. These can be initialised with other fields already established in `Main()` (e.g. myAccount, etc.)
```C#
string fromAccountName = "";
string toAccountName = "";
float transferAmount = 1.0f;
```

### 3. Replace existing do..while loop
The existing `do..while` loop used to prompt for user input via the console is no longer required and can, therefore, be commented out.  In its place, a new `while` loop associated with the window close requested event can be introduced to ensure the window continues to be rendered until explicitly closed (or the program is otherwise terminated).
```C#
while (!window.CloseRequested)
{
  // get user events...
  SplashKit.ProcessEvents();
  window.Clear(Color.White);
  
  // UI code will go here...
}
SplashKit.CloseAllWindows();
```

### 4. Create a container for the UI elements
While UI elements can be drawn directly in the window, SplashKit provides automatic layout functionality (i.e. positioning of elements in rows and columns) through use of containers. This, therefore, forms the starting point for the BankProgram UI.
```C#
while (!window.CloseRequested)
{
  // get user events...
  SplashKit.ProcessEvents();
  window.Clear(Color.White);

  // render a panel called "Bank Program" and match window size
  if (SplashKit.StartPanel("Bank Program", SplashKit.RectangleFrom(0, 0, 800, 600)))
  {
    // UI will be progressively built-out inside the panel...

    // finish rendering the panel (NOTE: name parameter must match corresopnding 'StartPanel')
    SplashKit.EndPanel("Bank Program");
  }
```

### 5. Setup menu items
Although there are numerous ways to structure the BankProgram UI, this design makes use of SplashKit's `Treenode` element to organise form fields under each menu option. To do this, the program iterates over the `MenuOption` enum and for each, renders a new node.
```C#
while (!window.CloseRequested)
{
  SplashKit.ProcessEvents();
  window.Clear(Color.White);

  if (SplashKit.StartPanel("Bank Program", SplashKit.RectangleFrom(0, 0, 800, 600)))
  {
    // iterate over the MenuOption enum
    foreach (MenuOption option in Enum.GetValues(typeof(MenuOption)))
    {
      // for each option, render a new node based on its label
      if (SplashKit.StartTreenode(option.ToString()) {

        // UI elements for each menu option will go here...

        // as with the panel in step 4, we must explicitly finish rendering each node, ensuring
        // the name parameter matches the one passed to the corresponding 'StartTreenode'
        SplashKit.EndTreenode(option.ToString());
      }
    }
    SplashKit.EndPanel("Bank Program");
  }
```

### 6. Handle each menu option
As with the original implementation, a `switch` statement can be used to handle each menu option and determine what happens within its corresponding node.
```C#
while (!window.CloseRequested)
{
  SplashKit.ProcessEvents();
  window.Clear(Color.White);

  if (SplashKit.StartPanel("Bank Program", SplashKit.RectangleFrom(0, 0, 800, 600)))
  {
    foreach (MenuOption option in Enum.GetValues(typeof(MenuOption)))
    {
      if (SplashKit.StartTreenode(option.ToString()) {

        switch (option)
        {
          case MenuOption.NewAccount:
            // Add new account form elements go here...
            break;
          case MenuOption.Withdraw:
            // Withdraw form elements go here...
            break;
          case MenuOption.Deposit:
            // Deposit form elements go here...
            break;
          case MenuOption.Transfer:
            // Transfer form elements go here...
            break;
          case MenuOption.Print:
            // Print form elements go here...
            break;
          case MenuOption.Quit:
            // Quit button goes here...
            break;
        }
        SplashKit.EndTreenode(option.ToString());
      }
    }
    SplashKit.EndPanel("Bank Program");
  }
```

### 7. Add Transfer form elements
As noted above, this tutorial focuses on the 'Transfer' form. Others can be built out in a similar way. 

Fundamentally, the Transfer opertion requires support for the following:
- entry of an account name from which funds should be transferred
- entry of an account name to which funds should be transferred
- entry of a transfer amount
- a button to execute the operation once relevant inputs have been provided

Again, there are a variety of ways to structure the form; however, to illustrate some of SplashKit's deeper capabilities, the design will make use of custom layouts. 
> To aid readability, subsequent code blocks will omit all menu options other than Transfer from the switch statement
```C#
switch (option)
{
  case MenuOption.Transfer:
    // tell SplashKit we're rendering a custom layout comprising two columns
    // (elements will automatically stack in rows)
    SplashKit.StartCustomLayout();
    SplashKit.SplitIntoColumns(2);

    // tell SplashKit we'd like to render the first element in column 1
    SplashKit.EnterColumn();

    // add a text box to accommodate collection of 'from' account name
    // ----------------------------------------------------------------
    // NOTE: this is placed inside an 'Inset' (a rectangular container)  
    // because SplashKit appears to have trouble handling multiple text
    // boxes embedded directly in the same node
    // ----------------------------------------------------------------
    SplashKit.StartInset("From Inset", 40);

    // the textbox is associated with the 'fromAccountName' field established in step 2, allowing
    // for its value to be captured and used in subsequent method calls
    fromAccountName = SplashKit.TextBox("From", fromAccountName);
    SplashKit.EndInset("From Inset", 40);

    // tell SplashKit we'd like to leave column 1 and enter column 2
    SplashKit.LeaveColumn();
    SplashKit.EnterColumn();

    // add a text box to accommodate collection of 'to' account name
    SplashKit.Inset("To Inset", 40);
    toAccountName = SplashKit.TextBox("To", toAccountName);
    SplashKit.EndInset("To Inset", 40);

    // tell SplashKit we'd like to leave column 2 and return to the default single column layout
    // for subsequent elements
    SplashKit.LeaveColumn();
    SplashKit.ResetLayout();

    // add a slider to accommodate collection of transfer amount
    // ---------------------------------------------------------------- 
    // NOTE: as with the text boxes, we assign the slider to the 'transferAmount' 
    // field established in step 2 so we can capture the current value.
    // The 1.0f and 1000.0f parameters are arbitrary and indicate the slider 
    // should support values in this range only. These could be established as 
    // 'MinValue' and 'MaxValue' constants respectively to aid readability
    // ----------------------------------------------------------------
    SplashKit.StartInset("Amount Inset", 40);
    transferAmount = SplashKit.Slider("Amount", transferAmount, 1.0f, 1000.0f);
    SplashKit.EndInset("Amount Inset");

    // add a button to support execution of the transfer opertion
    if (SplashKit.Button("Execute Transfer")
    {
      // method call to initiate transfer...
    }
    break;
}
```

### 8. Update Transfer operation methods
The existing `DoTransfer` method embeds console prompts, which are no longer required given the form built in step 7. Instead, a method which acts on the inputs collected via the form is required. The core logic to actually execute the transfer is unchanged. 

Although the existing method can be modified direcly, a new `DoTransfer` method can also be introduced as it will have a different signature.

> NOTE: these methods assume the Bank object (`_bank`) has been added as a static member of `Program.cs`. If this is not the case, they should be updated to include the Bank object as an additional parameter
```C#
private static void DoTransfer(string fromAccountName, string toAccountName, float transferAmount)
{
  try {
    Account fromAccount = FindAccount(fromAccountName);
    Account toAccount = FindAccount(toAccountName);

    // if one or both accounts can't be found, terminate operation
    if (fromAccount == null || toAccount == null) return;

    // the transferAmount must be cast to a decimal as the slider used to collect the value uses the float datatype
    decimal amount = Convert.ToDecimal(transferAmount);

    // execute per current implementation
    TransferTransaction transaction = new TransferTransaction(fromAccount, toAccount, amount);
    _bank.ExecuteTransaction(transaction);
    transaction.Print();
  }
  catch (Exception e)
  {
    Console.WriteLine(e.Message);
  }
}
```

The above method uses a simplified `FindAccount` utility method, which omits the console prompts.
```C#
private static Account FindAccount(string accountName)
{
  Account result = _bank.GetAccount(accountName);

  if (result == null)
    Console.WriteLine($"No account found with name {accountName}");

  return result;
}
```

### 9. Add method call to 'Execute Transfer' button
Returning to the form from step 7, the revised `DoTransfer` method can now be invoked on button click, taking the form input values as parameters.
```C#
switch (option)
{
  case MenuOption.Transfer:
    // other form elements...

    if (SplashKit.Button("Execute Transfer")
    {
      // method call to initiate transfer...
      DoTransfer(fromAccountName, toAccountName, transferAmount);
    }
    break;
}
```

### 10. Tell SplashKit to render the interface
Moving outside the base UI panel (container), it is necessary to prompt SplashKit to render the interface and refresh the window on each iteration of the `while` loop.
```C#
while (!window.CloseRequested)
{
  if (SplashKit.StartPanel("Bank Program", SplashKit.RectangleFrom(0, 0, 800, 600)))
  {
    // form elements...
  }
  // execute on each iteration of the loop to render the UI
  SplashKit.DrawInterface();
  SplashKit.RefreshScreen();
  window.Refresh();
}
```

### 11. Run the program
When the program is run, the UI should now be rendered in a window and the form elements operable. When a transfer is initiated, relevant messaging will be written to the console per the current implementation.

![ui-tutorial-screenshot](https://github.com/user-attachments/assets/8996095b-171c-4847-8ccd-a9feea813f19)
