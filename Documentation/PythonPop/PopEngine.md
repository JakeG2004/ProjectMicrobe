# Python Population Engine

## Classes
* [Microbe](./Microbe.md)
* [Environment](./Environment.md)

## Overview
The population engine is responsible for using the [Microbe](./Microbe.md) and [Environment](./Environment.md) classes to simulate microbe life using a modified version of the [Lotka-Volterra Model](https://bio.libretexts.org/Courses/Gettysburg_College/01%3A_Ecology_for_All/15%3A_Competition/15.05%3A_Quantifying_Competition_Using_the_Lotka-Volterra_Model). It is capable of simulating for any amount of time, dyanimcally adding resources, creating or removing microbes, loading in preset configurations, editing environmental parameters and microbe populations, and graphing all of those things.

## Functionality
The basic functionality is as follows
1) Calculate the competition coefficients for all microbes against all other microbes.
2) For each microbe
    1) Calculate the carry capacity given what resources are available
    2) Calculate the net resource usage based on the current population and carry capacity
    3) Add resource usage to a buffer
    4) Calculate the growth of the microbe population
    5) Update the population of the microbe
3) Add the resource buffer which was updated by each of the microbes to the environment
4) Log the histories and add the refreshed resources to the environment
5) Increment the time step
6) Show the changes to the user through the GUI

This process is repeated as many times as the user would like.