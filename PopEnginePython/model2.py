import numpy as np
import matplotlib.pyplot as plt
from collections import defaultdict

class Microbe:
    def __init__(self, name, population, growth_rate, required_resources, produced_resources, required_cleanliness):
        """
        :param name: Name of the microbe species.
        :param population: Initial population size.
        :param growth_rate: Growth rate per step.
        :param required_resources: Dictionary {resource_name: consumption_rate}.
        :param produced_resources: Dictionary {resource_name: production_rate}.
        :param required_cleanliness: Cleanliness threshold for growth.
        """
        self.name = name
        self.population = population
        self.growth_rate = growth_rate
        self.required_resources = required_resources
        self.produced_resources = produced_resources
        self.required_cleanliness = required_cleanliness

        # Stats over time
        self.history = []
        self.k_history = []

        # Competitors and mutualists
        self.competitors = {}

    def add_competitor(self, other_microbe):
        """Add a competitor or mutualistic partner with appropriate coefficients."""
        shared_resources = set(self.required_resources.keys()).intersection(set(other_microbe.required_resources.keys()))

        competition_coefficients = []
        mutualism_coefficients = []

        for res in shared_resources:
            competition_coefficients.append(
                other_microbe.required_resources[res] / self.required_resources[res]
            )

        for res in self.required_resources:
            if res in other_microbe.produced_resources:
                mutualism_coefficients.append(
                    -self.required_resources[res] / (other_microbe.produced_resources[res] + 1e-6)
                )
        
        for res in other_microbe.required_resources:
            if res in self.produced_resources:
                mutualism_coefficients.append(
                    -other_microbe.required_resources[res] / (self.produced_resources[res] + 1e-6)
                )

        total_coefficient = (sum(competition_coefficients) / len(competition_coefficients) if competition_coefficients else 0) + \
                            (sum(mutualism_coefficients) / len(mutualism_coefficients) if mutualism_coefficients else 0)

        self.competitors[other_microbe] = total_coefficient

    def compute_growth(self, k_resources, cleanliness):
        """Calculate growth based on available resources and competition"""
        # Find the limiting factor in the required resources
        min_k = min(k_resources[res] for res in self.required_resources)

        # Cleanliness effect (gradual impact)
        cleanliness_effect = max(0, (cleanliness - self.required_cleanliness))

        # Competition effect (scaled by population)
        competition_effect = sum(self.competitors[m] * m.population for m in self.competitors)

        if min_k == 0:
            growth = -abs(self.growth_rate * self.population * cleanliness_effect)
            return growth

        # Logistic growth formula, adjusting for available resources, cleanliness, and competition
        growth = (self.growth_rate * self.population * (min_k - self.population - competition_effect) / min_k) * cleanliness_effect
        return growth

    def update_population(self, change):
        """Update population, ensuring it can't be negative"""
        self.population = max(0, self.population + change)
        self.history.append(self.population)

class Environment:
    def __init__(self, initial_resources, resource_replenish_rate, dirty_resources):
        """
        :param initial_resources: Dictionary {resource_name: starting_amount}.
        :param resource_replenish_rate: Dictionary {resource_name: replenish_rate}.
        :param dirty_resources: Set of resources that impact cleanliness.
        """
        self.resources = initial_resources
        self.resource_replenish_rate = resource_replenish_rate
        self.resource_history = {res: [] for res in initial_resources}

        self.cleanliness = 1.0  # Initial cleanliness
        self.dirty_resources = dirty_resources
        self.clean_history = []

    def compute_carrying_capacity(self, microbes):
        """Calculate k for each resource, ensuring finite values"""
        k_resources = {}
        for res in self.resources:
            total_consumption = sum(m.population * m.required_resources.get(res, 0) for m in microbes)
            if total_consumption == 0:
                k_resources[res] = max(self.resources[res], 1)
            else:
                k_resources[res] = self.resources[res] / total_consumption if total_consumption > 0 else 0
        return k_resources

    def add_resources(self, newResources, mPop):
        """Add resources produced by microbes to the environment."""
        for res in newResources:
            self.resources[res] += min(newResources[res] * mPop, self.resources[res])

    def update_resources(self, microbes):
        """Update resources and cleanliness in the environment."""
        total_resources = sum(self.resources.values())
        total_dirty_resources = 0

        # Calculate resource consumption and replenishment
        for res in self.resources:
            total_consumed = sum(m.population * m.required_resources.get(res, 0) for m in microbes)
            self.resources[res] = max(0, self.resources[res] - total_consumed + self.resource_replenish_rate.get(res, 0))
            self.resource_history[res].append(self.resources[res])

        # Calculate environmental cleanliness
        for dirty_res in self.dirty_resources:
            if dirty_res in self.resources:
                total_dirty_resources += self.resources[dirty_res]

        self.cleanliness = 1 - (total_dirty_resources / total_resources) if total_resources > 0 else 1
        self.clean_history.append(self.cleanliness)

# Environment
env = Environment(
    initial_resources={"Oxygen": 10, "Lead": 10},
    resource_replenish_rate={"Oxygen": 0.5, "Lead": 0.5},
    dirty_resources={"Lead", "Arsenic"}
)

microbes = [
    Microbe("s1", 3, 1.02, {"Lead": 2}, {"Oxygen": 2}, 0),
    Microbe("s2", 3, 1.02, {"Oxygen": 2}, {"Lead": 2}, 0)
]

# Add competition/mutualism interactions between microbes
for microbe in microbes:
    for competitor in microbes:
        if microbe != competitor:
            microbe.add_competitor(competitor)

# Simulate over time
time_steps = 100
time = np.arange(time_steps)

for _ in range(time_steps):
    k_resources = env.compute_carrying_capacity(microbes)

    # Step 1: Calculate population changes
    pop_changes = {}
    resource_changes = {res: 0 for res in env.resources}

    for microbe in microbes:
        min_k = min(k_resources[res] for res in microbe.required_resources)
        microbe.k_history.append(min_k)

        # Calculate growth
        pop_changes[microbe] = microbe.compute_growth(k_resources, env.cleanliness)

        # Calculate resource contributions
        if min_k > 0:
            for res, amount in microbe.produced_resources.items():
                resource_changes[res] += min_k * amount

    # Step 2: Update populations
    for microbe in microbes:
        microbe.update_population(pop_changes[microbe])

    # Step 3: Update resources
    for res, amount in resource_changes.items():
        env.resources[res] = min(env.resources[res] + amount, env.resources[res])

    # Update environmental resources and cleanliness
    env.update_resources(microbes)

    # Check if resources have run out
    if all(val <= 0 for val in env.resources.values()):
        break

# Plot Results
fig, ax = plt.subplots(2, 2, figsize=(12, 10))  # 2 rows, 2 columns

# --- Microbial Population Growth ---
for microbe in microbes:
    ax[0, 0].plot(time[:len(microbe.history)], microbe.history, label=microbe.name)
ax[0, 0].set_xlabel("Time")
ax[0, 0].set_ylabel("Population")
ax[0, 0].set_title("Microbial Growth Over Time")
ax[0, 0].legend()
ax[0, 0].grid()

# --- Resource Levels ---
for res in env.resources:
    ax[0, 1].plot(time[:len(env.resource_history[res])], env.resource_history[res], label=f"{res} Level")
ax[0, 1].set_xlabel("Time")
ax[0, 1].set_ylabel("Resource Amount")
ax[0, 1].set_title("Resource Levels Over Time")
ax[0, 1].legend()
ax[0, 1].grid()

# --- Environmental Cleanliness ---
ax[1, 0].plot(time[:len(env.clean_history)], env.clean_history, label="Cleanliness", color='brown')
ax[1, 0].set_xlabel("Time")
ax[1, 0].set_ylabel("Cleanliness")
ax[1, 0].set_title("Environmental Cleanliness Over Time")
ax[1, 0].legend()
ax[1, 0].grid()

# --- Carrying Capacity (k-values) ---
for microbe in microbes:
    ax[1, 1].plot(time[:len(microbe.k_history)], microbe.k_history, label=microbe.name)
ax[1, 1].set_xlabel("Time")
ax[1, 1].set_ylabel("Carrying Capacity (k)")
ax[1, 1].set_title("Carrying Capacity Over Time")
ax[1, 1].legend()
ax[1, 1].grid()

plt.tight_layout()
plt.show()
