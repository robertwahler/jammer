Jammer
======

A Unity game jam template

Features
--------

* Loosely coupled, type safe event system

Installation
------------

Unity and vendor assets want to control their own line endings.
This mean turning off autocrlf is the path of least resistance.

### Clone jammmer

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

### Running tests from the command line

All command line test scripts require a Ruby Thor scripting environment with
Ruby > 2.0. These commands are configured for execution on Mac OS X. Other
environments will need to modify the Ruby source in the ./tasks folder.

    gem install bundler
    bundle install

#### Using Unity to run the tests (SLOW)

You need to shutdown the Unity IDE to run this command.

    thor test:unit

#### Using NUnit-console (FAST)

The Unity IDE can be running.

    thor test:nunit

#### Using Guard and the NUnit-console (CI)

This command will watch for file changes and automatically run the unit test
suite. The Unity IDE can be running.

    bundle exec guard

Syntax checking with Vim
------------------------

Do you use Vim instead of MonoDevelop/Visual Studio? Install
https://github.com/neomake/neomake and add this to your .vimrc

    let g:neomake_cs_mcs_maker = {
      \ 'args': ['@.mcs'],
      \ 'errorformat': '%f(%l\,%c): %trror %m',
      \ }

License
-------

MIT, see ./LICENSE for details.
