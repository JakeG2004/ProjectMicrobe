import matplotlib.pyplot as plt
import numpy as np

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

        # Set up arrays to keep track of history (for graphing :3)
        self.pop_history = []
        self.k_history = []

        # List of competitors to calculate competition coefficients
        self.competitors = {}

    # Use the Lotka-Volterra model for this
    def compute_growth(self, k_resources):
        # Determine the limiting resource
        min_k = min(k_resources[res] for res in self.required_resources)

        # Avoid division by zero if resources are depleted
        if min_k == 0:
            return self.growth_rate * -0.33 * self.population  # Extinction effect

        # Compute competition effect
        competition_effect = sum(self.competitors[m] for m in self.competitors)

        #print(self.competitors)
        growth = self.growth_rate * self.population * (1 - (competition_effect / min_k))
        #print(growth)

        return max(growth, -self.population)  # Prevent overshooting negative population

    
    def update_population(self, new_pop):
        self.pop_history.append(self.population)
        self.population = max(0, self.population + new_pop)

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
           

class Environment:
    def __init__(self, initial_resources, resource_refresh_rate):
        self.resources = initial_resources
        self.resource_refresh_rate = resource_refresh_rate

        self.resource_history = {res: [] for res in initial_resources}

    def compute_carry_capacity(self, microbe):
        k_resources = {}

        for res in self.resources:
            resource_consumption = microbe.required_resources.get(res, 0)  # Per microbe consumption

            if(microbe.population == 0): # If there is no population, report k = 0
                k_resources[res] = 0
            elif resource_consumption == 0:
                k_resources[res] = float('inf')  # If the microbe doesn't use this resource, no limit
            else:
                k_resources[res] = self.resources[res] / resource_consumption

        return k_resources

    
    def update_resources(self, microbes):
        for res in self.resources:
            # Log resources
            self.resource_history[res].append(self.resources[res])

            # update resource usage
            total_consumed = sum(m.population * m.required_resources.get(res, 0) for m in microbes)
            self.resources[res] = max(0, self.resources[res] - total_consumed + self.resource_refresh_rate.get(res, 0))

env = Environment(
    initial_resources={"Oxygen": 3},
    resource_refresh_rate={"Oxygen": 2}
)

microbes = [
    Microbe("s1", 1, 1.01, {"Oxygen": 1}, {}, {}),
    Microbe("s2", 2, 1.01, {"Oxygen": 2}, {}, {})
]

# Calculate competition coefficients for microbes
for m1 in microbes:
    for m2 in microbes:
        m1.add_competitor(m2)

# Begin simulation
time_steps = 25

for time in range(time_steps):
    # Recalculate competition coefficients at every time step
    for m1 in microbes:
        m1.competitors = {}
        for m2 in microbes:
            m1.add_competitor(m2)

    for microbe in microbes:
        # Calculate carrying capacity
        k_resources = env.compute_carry_capacity(microbe)
        min_k = min(k_resources[res] for res in microbe.required_resources)

        # Log carrying capacity
        microbe.k_history.append(min_k)

        # Calculate new microbe population
        pop_change = microbe.compute_growth(k_resources)
        microbe.update_population(pop_change)

    # Mass update resources
    env.update_resources(microbes)

    # Break if no more resources
    if(all(val <= 0 for val in env.resources.values())):
        break

def moving_average(data, window_size=5):
    return np.convolve(data, np.ones(window_size)/window_size, mode='valid')

fig, ax = plt.subplots(1, 2, figsize=(12, 5))
window_size=1

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

plt.tight_layout()
plt.show()