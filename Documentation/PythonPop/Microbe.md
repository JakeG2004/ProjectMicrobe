# Microbe Class

## Overview
The microbe class is used by the [Population Engine](./PopEngine.md) to simluate growth and interaction between several microbes and the [Environment](./Environment.md).

## Fields
* **String name**: Stores the name of the microbe
* **Float population**: Stores the current population of a microbe
* **Dict competitors**: Stores a dictionary of competitors in the form (competitor name, competition coefficient)
* **Dict required_resources**: Stores the required resources in the form (resource name, amount needed)
* **Dict produce_resources**: Stores the produced resources in the form (resource name, amount produced)
* **Dict toxins**: Stores the toxins in the form (toxin name, toxicity dictionary)
    * The toxicity dictionary is in the form (toxicity level, min safe density, max safe density, lethal density). These numbers are per microbe.
* **Dict k_resources**: Stores the carrying capacity for each resource in the form (resource name, carrying capacity)
* **List pop_history**: A list which stores the population of the microbe in each time step
* **List k_history**: A list which stores the carrying capacity of the microbe in each time step

## Methods
### Constructor(name, initial_population, growth_rate, required_resources, produced_resources, toxins)
Creates a new microbe with the data entered in the arguments.

### compute_growth()
This function is responsible for calculating the growth of a microbe given the resources in its environment. It uses the [Lotka-Volterra Model](https://bio.libretexts.org/Courses/Gettysburg_College/01%3A_Ecology_for_All/15%3A_Competition/15.05%3A_Quantifying_Competition_Using_the_Lotka-Volterra_Model) as described on libretexts.org.

The function follows this basic sequence
1) Find the minimum in k_resources
2) If the minimum is 0, reduce the population by 1/3 and early return
3) Otherwise, sum the competition effect given by all of the microbe's competitors
4) Calculate growth using the Lotka-Volterra Model.

### calculate_toxicity_multiplier()
In this simulation, the toxicity in an environment is factored in as a multiplier to the carring capacity of every resource. This multiplier uses the relative density of a material in the resource pool. A resource that is more toxic is weighted to have a higher density. This function is responsible for calculating what that multiplier should be.

The function follows this basic sequence
1) Get the total amount of resources
2) For every resource that is toxic to the microbe
    1) Find the weighted density of the given resource, given by `material toxicity * toxic resource amount / total amount of resources`
    2) If toxicity is below the safe threshold, continue to next resource
    3) If toxicity is above the safe threshold, return 1
    4) Otherwise, normalize the toxicity to a value between 0 and 1. Store this value
3) Return the minimum of the calculated toxicity levels

### update_population(new_pop)
This function records the microbe's current population into its pop_history dictionary. It then calculates the new population size given new_pop.

### add_competitor(other_microbe)
This function is responsible for finding the competition coefficient between two microbes, and storing it in the competitors dictionary.

The function follows this basic sequence
1) Find the shared resources between this microbe and the other microbe
2) For each resource that is shared, calculate the competition coefficient for this resource as `other_microbe amount required / this_microbe amount required`
3) Store the microbe competition coefficient in the competitors dictionary as `other_microbe population * maximum of the resource compeition coefficients`

### produce_consume_resources()
This function is responsible for calculating the net usage of resources by this microbe.

The function follows this basic sequence
1) Find the minimum in k_resources
2) For each resource in required_resources, add the resource consumption to the resource tracker as `required amount for resource * -1 * min(microbe population, carry capacity)`.
3) For each resource in produced_resources, add the resource production to the resource tracker as `amount produced for resource * min(microbe population, carry capacity)`

### compute_carry_capacity(environment resources)
This function calculates the carrying capacity of every environment in the resource for the given microbe. Additionally, it factors in environmental toxicity as a multiplier to the carry capacity.

The function follows this basic sequence
1) call `calculate_toxicity_multiplier()` and store
2) For each resource, store the corresponding value in k_resources as `toxicity multiplier * # of resource in environment  / microbe consumption of this resource`
3) Append the minimum in k_resources to k_history