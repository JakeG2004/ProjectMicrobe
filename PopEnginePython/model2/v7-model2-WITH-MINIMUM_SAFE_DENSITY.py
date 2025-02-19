import matplotlib.pyplot as plt
import numpy as np
import math

# Define our classes
class Microbe:
    def __init__(self, name, initial_population, growth_rate, required_resources, produced_resources, toxins):
        # Set up initial values
        self.name = name
        self.population = initial_population
        self.growth_rate = growth_rate
        self.required_resources = required_resources
        self.produced_resources = produced_resources
        self.toxins = toxins

        # Carry capacity
        self.k_resources = {}

        # Set up arrays to keep track of history (for graphing :3)
        self.pop_history = []
        self.k_history = []

        # List of competitors to calculate competition coefficients
        self.competitors = {}

    # Use the Lotka-Volterra model for this
    def compute_growth(self):
        # Determine the limiting resource
        min_k = min(self.k_resources[res] for res in self.required_resources)

        # Avoid division by zero if resources are depleted
        if min_k == 0:
            if(self.population <= 2):
                return -1 * self.population # Kill population if small enough
            return self.growth_rate * -0.33 * self.population  # Extinction effect

        # Compute competition effect
        competition_effect = sum(self.competitors[m] for m in self.competitors)

        growth = self.growth_rate * self.population * (1 - (competition_effect / min_k))

        return max(growth, -self.population)  # Prevent overshooting negative population
    
    def calculate_toxicity_multiplier(self, env_resources):
        # Find total resources 
        total_resources = sum(env_resources.values())
        min_toxicity = 1.0

        for res in self.toxins:
            # Find weighted density of the current toxin
            cur_toxin = self.toxins[res]
            max_safe_density = cur_toxin["max_safe_density"]
            min_safe_density = cur_toxin["min_safe_density"]
            lethal_density = cur_toxin["lethal_density"]
            toxin_weighted_density = (cur_toxin["toxicity"] * env_resources[res]) / total_resources

            # If the density is in the safe range, continue to next toxin
            if(toxin_weighted_density <= max_safe_density and toxin_weighted_density >= min_safe_density):
                continue
            
            # If the density is lethal, return 0
            if(toxin_weighted_density >= lethal_density):
                return 0.0
            
            # If the density is between lethal and max safe density, then normalize the toxicity between the two
            if(toxin_weighted_density >= max_safe_density and toxin_weighted_density <= lethal_density):
                cur_toxicity = (toxin_weighted_density - lethal_density) / (max_safe_density - lethal_density)
            
            # If the density is less than the minimum that is safe, 
            if(toxin_weighted_density <= min_safe_density):
                cur_toxicity = (toxin_weighted_density) / (min_safe_density)
            
            # Keep track of the most toxic thing
            min_toxicity = min(min_toxicity, cur_toxicity)

        return min_toxicity

    # Update the population and history
    def update_population(self, new_pop):
        self.pop_history.append(self.population)
        self.population = max(0, self.population + new_pop)

    # Calculate competition between this microbe and other microbes
    def add_competitor(self, other_microbe):
        shared_resources = set(self.required_resources.keys()).intersection(set(other_microbe.required_resources.keys()))

        competition_coefficients = []

        # Calculate competition coefficient based on other consumption / self consumption
        for res in shared_resources:
            competition_coefficients.append(other_microbe.required_resources[res] / self.required_resources[res])

        # Competition coefficient is determined to be the mean of all resources they are competing for
        if not competition_coefficients:
            self.competitors[other_microbe] = 0
        else:
            self.competitors[other_microbe] = other_microbe.population * sum(competition_coefficients) / len(competition_coefficients)
    
    # Calculate the net resource change from a microbe in one time step
    def produce_consume_resources(self):
        # Determine the limiting resource
        min_k = min(self.k_resources[res] for res in self.required_resources)

        resource_change = {}

        # Add the consumed resources
        for res in self.required_resources:
            resource_change[res] = self.required_resources[res] * min_k * -1

        # Add the produced resources
        for res in self.produced_resources:
            if res in resource_change:
                resource_change[res] += self.produced_resources[res] * min_k
            else: # Add new element to dict if needed
                resource_change[res] = self.produced_resources[res] * min_k

        return resource_change
    
    # Calculate the carry capacity for a given microbe
    def compute_carry_capacity(self, env_resources):

        # Find minimum toxicity multiplier
        toxicity_mult = self.calculate_toxicity_multiplier(env_resources)

        # Find the carry capacity
        for res in env_resources:
            resource_consumption = self.required_resources.get(res, 0)  # Per microbe consumption

            if(self.population == 0): # If there is no population, report k = 0
                self.k_resources[res] = 0
            elif resource_consumption == 0:
                self.k_resources[res] = float('inf')  # If the microbe doesn't use this resource, no limit
            else:
                self.k_resources[res] = (env_resources[res] / resource_consumption) * toxicity_mult
        
        # Append the minimum to the history
        min_k = min(self.k_resources[res] for res in self.required_resources)
        self.k_history.append(min_k)


class Environment:
    def __init__(self, initial_resources, resource_refresh_rate):
        self.resources = initial_resources
        self.resource_refresh_rate = resource_refresh_rate

        self.resource_history = {res: [] for res in initial_resources}

    # Record resource history
    def update_resource_history(self):
        for res in self.resources:
            # Log resources
            self.resource_history[res].append(self.resources[res])

            # update resource usage
            self.resources[res] = max(0, self.resources[res] + self.resource_refresh_rate.get(res, 0))

    # Add resources to the environment
    def add_resources(self, added_resources):
        for res in added_resources:
            self.resources[res] += added_resources[res]

env = Environment(
    initial_resources={"Oxygen": 10, "Glucose": 2, "Lead": 0},
    resource_refresh_rate={"Oxygen": 10, "Glucose": 1, "Lead": 1}
)

microbes = [
    Microbe(
        name="m1", 
        initial_population=1, 
        growth_rate=1.01, 
        required_resources={"Oxygen": 1,}, 
        produced_resources={}, 
        toxins={
            "Lead": {
                "toxicity": 1, 
                "min_safe_density": 0.0,
                "max_safe_density": 0.4,
                "lethal_density": 0.6},
            "Glucose": {
                "toxicity": 1,
                "min_safe_density": 0.2,
                "max_safe_density": 0.4,
                "lethal_density": 1.0},
        },),
]

# Begin simulation
time_steps = 10

for time in range(time_steps):
    # Recalculate competition coefficients at every time step because population is part of the calculations
    for m1 in microbes:
        m1.competitors = {}
        for m2 in microbes:
            m1.add_competitor(m2)

    for microbe in microbes:
        # Get carry capacity of microbe
        microbe.compute_carry_capacity(env.resources)

        # Get resource changes due to microbe
        net_resource_usage = microbe.produce_consume_resources()
        env.add_resources(net_resource_usage)

        # Calculate new microbe population
        pop_change = microbe.compute_growth()
        microbe.update_population(pop_change)

    # Log resource history
    env.update_resource_history()

    # Break if no more resources
    if(all(val <= 0 for val in env.resources.values())):
        break

# Smoothing on graphical representation
def moving_average(data, window_size=5):
    return np.convolve(data, np.ones(window_size)/window_size, mode='valid')

fig, ax = plt.subplots(1, 3, figsize=(18, 5))
window_size=3

# Microbes
for microbe in microbes:
    smoothed_pop = moving_average(microbe.pop_history, window_size)
    ax[0].plot(range(len(smoothed_pop)), smoothed_pop, label=microbe.name)

ax[0].set_xlabel("Time")
ax[0].set_ylabel("Population")
ax[0].set_title("Smoothed Microbial Growth Over Time")
ax[0].legend()
ax[0].grid()

# Carrying capacity
for microbe in microbes:
    smoothed_k = moving_average(microbe.k_history, window_size)
    ax[1].plot(range(len(smoothed_k)), smoothed_k, label=microbe.name)

ax[1].set_xlabel("Time")
ax[1].set_ylabel("Carrying Capacity")
ax[1].set_title("Smoothed Carrying Capacity Over Time")
ax[1].legend()
ax[1].grid()

# Resource Levels Over Time
for resource, values in env.resource_history.items():
    ax[2].plot(range(len(values)), values, label=resource)

ax[2].set_xlabel("Time")
ax[2].set_ylabel("Resource Level")
ax[2].set_title("Resource Levels Over Time")
ax[2].legend()
ax[2].grid()


plt.tight_layout()
plt.show()