Jammer
======

A Unity game jam template

Features
--------

* Loosely coupled, type safe event system
* Lighting fast syntax checking for Vim users

Assets
------

Several free and open source vendor assets are included. Most game types will
benefit from all of these vendor assets. None are required and can be removed
be deleting the appropriate folder from `./Assets/Plugins/Vendor`. All can be
redistributed with your source.

* UnityTestTools for unit and integration testing. https://www.assetstore.unity3d.com/en/#!/content/13802
* ConsoleE Free. A replacement Unity debug trace console. https://www.assetstore.unity3d.com/en/#!/content/42381

### Post Jam Assets

Cleaning up your game after the Jam? These non-free assets should will make any
Unity Dev happy. Alas, you can't redistribute these asset binaries or source.

* ConsoleE Pro.  Like the free one above but better. https://www.assetstore.unity3d.com/en/#!/content/11521
* Text Mesh Pro. SDF Fonts! Seeing is believing. This asset should be part of stock Unity. https://www.assetstore.unity3d.com/en/#!/content/17662

Installation
------------

Unity and vendor assets want to control their own line endings.
This mean turning off autocrlf is the path of least resistance.

### Clone Jammmer

    cd ~/workspace
    git clone git@github.com/robertwahler/jammer.git my_game -n
    cd my_game 
    git config core.autocrlf false
    git checkout

Testing
-------

The NUnit test framework is included in Unity 5.3 and higher.  Tests require
installation of the UnityTestTools asset for Unity 5.2 and lower.

### Running tests from Unity IDE

There is no Unity hotkey for running tests. Instead, manually use this menu sequence:

    Main Menu: Window, Editor Tests Runner
    Editor Tests: Run All

#### Using Guard and the NUnit-console (CI)

This command will watch for file changes and automatically run the unit test
suite. The Unity IDE can be running.

    bundle exec guard

Syntax checking with Vim
------------------------

Do you use Vim instead of MonoDevelop/Visual Studio? 

Install https://github.com/neomake/neomake and add this to your .vimrc

    let g:neomake_cs_mcs_maker = {
      \ 'args': ['@.mcs'],
      \ 'errorformat': '%f(%l\,%c): %trror %m',
      \ }

Errors can be viewed via :lopen

### Create .mcs file

    thor compile:mcs

License
-------

MIT, see ./LICENSE for details.
