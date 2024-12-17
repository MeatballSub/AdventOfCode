#!/bin/bash
#
aocrun () 
{ 
    ( cd /D/Win11/source/repos/AdventOfCode/2024/Day$1;
    time ( clear;
    winpty dotnet run --configuration=release ) )
}
