import matplotlib.pyplot as plt
import numpy as np
import random

# Define our classes
class Microbe:
    def __init__(self, name, initial_population, growth_rate, required_resources, produced_resources, toxins, lethal_toxicity, safe_toxicity):
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

        # Cleanliness variables
        self.env_cleanliness = 1
        self.lethal_toxicity = lethal_toxicity
        self.safe_toxicity = safe_toxicity

    # Use the Lotka-Volterra model for this
    def compute_growth(self, k_resources):
        # Determine the limiting resource
        min_k = min(k_resources[res] for res in self.required_resources)

        # Avoid division by zero if resources are depleted
        if min_k == 0:
            if(self.population <= 2):
                return -1 * self.population # Kill population if small enough
            return self.growth_rate * -0.33 * self.population  # Extinction effect

        # Compute competition effect
        competition_effect = sum(self.competitors[m] for m in self.competitors)

        growth = self.growth_rate * self.population * (1 - (competition_effect / min_k))

        return max(growth, -self.population)  # Prevent overshooting negative population
    
    # Linear probability of survival between lethal and safe toxicity
    def calculate_toxin_survival_chance(self):
        if(self.env_cleanliness >= self.safe_toxicity):
            return 1
        if(self.env_cleanliness < self.lethal_toxicity):
            return 0
        
        # Survival probability = normalized value between lethal and safe toxicity
        survival_prob = (self.env_cleanliness - self.lethal_toxicity) / (self.safe_toxicity - self.lethal_toxicity)

        return random.random() < survival_prob

    def calculate_environmental_cleanliness(self, env_resources):
        # Get sum of all resources
        total_resources = sum(env_resources[res] for res in env_resources)

        # Early return to avoid div by 0
        if(total_resources == 0):
            return 1

        # Get sum of all toxic resources
        toxic_resources = sum(env_resources.get(t_res, 0) for t_res in self.toxins)

        # Cleanliness = toxic resources / total resources
        self.env_cleanliness = 1 - (toxic_resources / total_resources)

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
    def produce_consume_resources(self, k_resources):
        # Determine the limiting resource
        min_k = min(k_resources[res] for res in self.required_resources)

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


class Environment:
    def __init__(self, initial_resources, resource_refresh_rate):
        self.resources = initial_resources
        self.resource_refresh_rate = resource_refresh_rate

        self.resource_history = {res: [] for res in initial_resources}

    # Calculate the carry capacity for a given microbe
    def compute_carry_capacity(self, microbe):
        k_resources = {}

        # Account for toxicity
        survival_chance = microbe.calculate_toxin_survival_chance()

        for res in self.resources:
            resource_consumption = microbe.required_resources.get(res, 0)  # Per microbe consumption

            if(microbe.population == 0): # If there is no population, report k = 0
                k_resources[res] = 0
            elif resource_consumption == 0:
                k_resources[res] = float('inf')  # If the microbe doesn't use this resource, no limit
            else:
                k_resources[res] = (self.resources[res] / resource_consumption) * survival_chance

        return k_resources

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
    initial_resources={"Oxygen": 100, "Glucose": 0, "Lead": 0},
    resource_refresh_rate={"Oxygen": 0, "Glucose": 0, "Lead": 1}
)

microbes = [
    Microbe("s1", 1, 1.01, {"Oxygen": 1}, {"Glucose": 1}, {}, .4, .6),
    Microbe("s2", 1, 1.01, {"Glucose": 1}, {"Oxygen": 1}, {"Lead": 1}, .4, .6)
]

# Calculate competition coefficients for microbes
for m1 in microbes:
    for m2 in microbes:
        m1.add_competitor(m2)

# Begin simulation
time_steps = 135

for time in range(time_steps):
    # Recalculate competition coefficients at every time step because population is part of the calculations
    for m1 in microbes:
        m1.competitors = {}
        for m2 in microbes:
            m1.add_competitor(m2)

    for microbe in microbes:
        # Get the environmental cleanliness on a per microbe basis
        microbe.calculate_environmental_cleanliness(env.resources)

        # Calculate carrying capacity
        k_resources = env.compute_carry_capacity(microbe)
        min_k = min(k_resources[res] for res in microbe.required_resources)

        # Log carrying capacity
        microbe.k_history.append(min_k)

        # Get resource changes due to microbe
        resource_usage = microbe.produce_consume_resources(k_resources)
        env.add_resources(resource_usage)

        # Calculate new microbe population
        pop_change = microbe.compute_growth(k_resources)
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