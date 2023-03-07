# CM3030 - Game Development Project Readme

## Team and Role Assignments
| Team member | Role | Git User Id |
| ----------- | ---- | ----------- |
| Hamish Grigor | Lead Designer, Producer and Game Mechanics Code | hbgr |
| Johann Ungerer | Lead Developer, Co-Producer and VFX | Kaegun |
| Marianna Siembiot | Art and Sound Direction | marcepannam |
| Raj Maurya | Level Design | RajGM |
| Xiyu Zhou (Kathy) | **Absconded** | HelloCat8 |

# REFERENCES
[a relative link](REFERENCES.md)

## Software and Tools
1. Unity 2019.4.40f LTS
   1. We are using the latest available release of Unity as at the start of the team project. Ideally we should now stick to this version for the entire build cycle, unless we find a compelling reason to upgrade such as a major bug fix, etc.
1. OneDrive - https://1drv.ms/u/s!Aogm4S-HQ3gila0_w627oEFvd0XuQg?e=zkexuJ
   1. We will place all working documents on OneDrive, enabling us to edit Word documents jointly and easily share content that is better suited to a file system rather than a code repository.
1. Codecks - https://uol-cm3030.codecks.io/decks
   1. We will use Codecks to track tasks. It is a neat task tracking platform, built for Game Development.
1. Github - https://github.com/Kaegun/uol-cm3030/
   1. We'll use GitHub for our version control. An appropriate gitignore has been configured and LFS support for common Unity and asset file types has been enabled.
   1. See here - https://git-lfs.github.com/, for LFS guidance

## How to set up your git environment:
1. First install git from here: https://git-scm.com/downloads
1. Then install git LFS: https://git-lfs.github.com/
1. Run git lfs install to initialize git large file support
1. Clone the repo into your working folder location using git clone https://github.com/Kaegun/uol-cm3030.git
1. You can now open the Unity projects that have been created in the repo
1. No scene will be loaded by default, so open a scene from the Scenes folder.

## Coding Standards
We should all try and follow a consistent coding standard. C# and Unity tend to adhere to the following strictures:
* Use PascalCase for method names, e.g.
```cs
private void SomeMethod(int someVariable)
{
   //  Do some stuff
}
```
* Variable names should use camelCase, e.g.
```cs
var myVariable = 0.0f;
```
* Use an underscore `_` to denote a class global variable, e.g.
```cs
private float _someClassVariable = 0.0f;
```
* Use the `[SerializeField]` attribute when exposing varaibles to the Unity Editor, instead of making variables public, e.g.
```cs
[SerializeField]
private float _editorVariable = 0.0f;
```
* Line spacing - Please do not leave multiple empty lines in the code.
* General Formatting - Use Ctrl K + D to auto format your code to the predefined formatting settings configured.

For now, that'll do, we can get more specific as use cases arrive.

***As always, Happy coding!***