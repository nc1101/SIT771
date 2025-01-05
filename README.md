# Tutorial - Bank Program UI with SplashKit

## Background
This brief tutorial seeks to demonstrate how SplashKit can be used to implement a simple UI for the BankProgram being built out in SIT771. The codebase used as the basis for the tutorial (and related testing) included all relevant tasks up to (and including '5.3 Many Accounts'.

## Objectives
This tutorial will demonstrate:
- a basic approach to using SplashKit's Interface elements and functions
- a practical example of a form for the 'Transfer' operation
- how BankProgram methods can be adapted to act on UI inputs

## Tutorial

> All changes discussed in this section are to `Program.cs`.

### 1. Create a window
The first step in introducing a UI to the BankProgram application is to create a window in which to render the interface (as demonstrated in other programs throughout the semester).
```C#
Window window = new Window("Bank Program", 800, 600);
```

### 2. Create fields to accommodate form inputs
As the tutorial is focused on the 'Transfer' operation, three fields are required to accommodate user inputs. These can be initialised with other fields already established in `Main()` (e.g. bank, account, etc.)
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
```

### 4. Create a container for the UI elements
While SplashKit UI elements can be drawn directly in the window, it provides automatic layout functionality (i.e. positioning of elements in rows and columns) through use of containers. This, therefore, forms the starting point for the BankProgram UI.
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
