import numpy as np
import matplotlib.pyplot as plt

class Microbe:
    def __init__(self, name, population, growth_rate, required_resources, produced_resources):
        """
        :param name: Name of the microbe species.
        :param population: Initial population size.
        :param growth_rate: Growth rate per step.
        :param required_resources: Dictionary {resource_name: consumption_rate}.
        """
        # Set up initial variables
        self.name = name
        self.population = population
        self.growth_rate = growth_rate
        self.required_resources = required_resources
        self.produced_resources = produced_resources

        # Stats over time
        self.history = []
        self.k_history = []

        # Competitor list
        self.competitors = {}

    def add_competitor(self, other_microbe):
        """Add a competitor with competition coefficient based on shared resources."""
        # Find the shared resources between two microbes
        shared_resources = set(self.required_resources.keys()).intersection(set(other_microbe.required_resources.keys()))

        # Calculate competition coefficient for each shared resource
        competition_coefficients = []
        for res in shared_resources:
            competition_coefficients.append(
                other_microbe.required_resources[res] / self.required_resources[res]
            )
        
        # Average competition coefficient
        if competition_coefficients:
            self.competitors[other_microbe] = sum(competition_coefficients) / len(competition_coefficients)
        else:
            self.competitors[other_microbe] = 0  # No shared resources = no competition

    def compute_growth(self, k_resources):
        """Calculate growth based on available resources and competition"""
        # Find the limiting factor in the required resources
        min_k = min(k_resources[res] for res in self.required_resources)

        # Calculate the competition effect as a single value. a(x,y) * N in the model
        competition_effect = sum(self.competitors[m] * m.population for m in self.competitors)

        # Complete the rest of the model

        if(min_k == 0):
            return 0
        
        return(self.growth_rate * self.population) * (min_k - self.population - competition_effect) / min_k

    def update_population(self, change):
        """Update pop, no neg"""
        self.population = max(0, self.population + change)
        self.history.append(self.population)

class Environment:
    def __init__(self, initial_resources, resource_replenish_rate):
        """
        :param initial_resources: Dictionary {resource_name: starting_amount}.
        :param resource_replenish_rates: Dictionary {resource_name: replenish_rate}.
        """
        self.resources = initial_resources
        self.resource_replenish_rate = resource_replenish_rate
        self.resource_history = {res: [] for res in initial_resources}

    def compute_carrying_capacity(self, microbes):
        """Calculate k for each resource"""
        k_resources = {}
        for res in self.resources:
            # Get the total consumption of the current resource from every microbe
            total_consumption = sum(m.population * m.required_resources.get(res, 0) for m in microbes)

            # Handle divide by zero if necessary
            if(total_consumption == 0):
                k_resources[res] = float('inf')
            else:
                k_resources[res] = self.resources[res] / total_consumption

        return k_resources
    
    def add_resources(self, newResources, mPop):
        for res in newResources:
            # Add the resources produced by the microbe, but do not exceed the original amount
            self.resources[res] += min(newResources[res] * mPop, self.resources[res])

    def update_resources(self, microbes):
        for res in self.resources:
            # Calculate how much of that resource has been consumed
            total_consumed = sum(m.population * m.required_resources.get(res, 0) for m in microbes)

           # Set the new resources to reflect the change
            self.resources[res] = max(0, self.resources[res] - total_consumed + self.resource_replenish_rate.get(res, 0))

            # Update history
            self.resource_history[res].append(self.resources[res])

# Init env and microbes
env = Environment(
    initial_resources={"Oxygen": 50, "Glucose": 25},
    resource_replenish_rate={"Oxygen": 0, "Glucose": 0}
)

microbes = [
    Microbe("s1", 1, 1.01, {"Oxygen": 2}, {"Glucose": 1}),
    Microbe("s2", 2, 1.03, {"Glucose": 1}, {"Oxygen": 5})
]

# Make all microbes compete
for microbe in microbes:
    for competitor in microbes:
        if microbe != competitor:
            microbe.add_competitor(competitor)

# ===== SIMULATE =====
time_steps = 250
time = np.arange(time_steps)

for _ in range(time_steps):
    k_resources = env.compute_carrying_capacity(microbes)

    for microbe in microbes:
        min_k = min(k_resources[res] for res in microbe.required_resources)
        microbe.k_history.append(min_k)

        # If the microbe is able to consume and produce resources
        if(min_k > 0):
            env.add_resources(microbe.produced_resources, min(min_k, microbe.population))


        change_pop = microbe.compute_growth(k_resources)
        microbe.update_population(change_pop)

    env.update_resources(microbes)

    # Check that all resources are empty before breaking
    if(all(val <= 0 for val in env.resources.values())):
        break

# === Plot Results (Both Graphs at Once) ===
fig, ax = plt.subplots(1, 2, figsize=(12, 5))  # Side-by-side plots

# --- Plot Microbial Population Growth ---
for microbe in microbes:
    ax[0].plot(time[:len(microbe.history)], microbe.history, label=microbe.name)

ax[0].set_xlabel("Time")
ax[0].set_ylabel("Population")
ax[0].set_title("Microbial Growth Over Time")
ax[0].legend()
ax[0].grid()
ax[0].set_ylim(0, max(max(m.history) for m in microbes) * 1.2)

# --- Plot Resource Levels ---
for res in env.resources:
    ax[1].plot(time[:len(env.resource_history[res])], env.resource_history[res], label=f"{res} Level")

ax[1].set_xlabel("Time")
ax[1].set_ylabel("Resource Amount")
ax[1].set_title("Resource Levels Over Time")
ax[1].legend()
ax[1].grid()

# Show both graphs at once
plt.tight_layout()
plt.show() 