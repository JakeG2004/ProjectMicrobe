# Environment Class
The environment class responsible for holding the resources that the [Microbes](./Microbe.md) will use

## Fields
* **Dict resources**: Stores resources as (resource name, amount)
* **Dict resource_refresh_rate**: Stores resource refresh rates as  (resource name, refresh amount)
* **Dict resource_history**: A dictionary storing resource history in the form (resource name, history array). The history array is what stores the data.

## Methods
### Constructor(initial_resources, resource_refresh_rate)
Takes in the prefilled dictionaries `initial_resources` and `resource_refresh_rate`, and constructs an environment object with those properties.

### update_resource_history()
This function is responsible logging the resources, as well as doing the resource refreshing in accordance with the refresh rates.

The basic steps are as follows
1) For each resource
    1) Append the current resource amount to `resource_history`, adding new entries as needed.
    2) Add the amount specified in the resource refresh rate

### add_resources(added_resources)
This function is responsible for taking in external resources as `dict added_resources` and adding them to the environment.