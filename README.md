# FileManager

![image](https://user-images.githubusercontent.com/43960228/190326170-19729cf4-c99e-46e4-89b5-2a9a186dd954.png)


## Function ## 

Functional requirements:
- Consists of a single area *("working area")*, which displays the contents of the current directory (or folder)
- Upon double clicking on an element:
  - If it is a *file*, then the application tries to open using windows;
  - If it is a *folder*, then the working area is filled with the contents of this *folder*
- Upon a single click on an element, on the right side of the working area, a panel should appear that displays *additional information*:
  - If it is a *file*, then its metadata is displayed *(size, date created, etc..)*
  - If it is a *folder*, then its size and amount of file it contains is displayed
- Upon opening a *file*, write into the database. The history entity contains:
  - Id
  - Filename
  - Date visited

## How to use ##
- Just Compile and run programm
