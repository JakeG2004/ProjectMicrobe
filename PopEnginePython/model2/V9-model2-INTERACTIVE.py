import matplotlib.pyplot as plt
import numpy as np

#
# --- MICROBE CLASS
#

class Microbe:
    def __init__(self, name, initial_population, growth_rate, required_resources, produced_resources, toxins):
        """Create a new Microbe population with given properties"""

        # Basics required for growth equation
        self.name = name
        self.population = initial_population
        self.growth_rate = growth_rate
        self.competitors = {}

        # Resources and toxins
        self.required_resources = required_resources
        self.produced_resources = produced_resources
        self.toxins = toxins

        # Carry capacity
        self.k_resources = {}

        # Set up arrays to keep track of history (for graphing :3)
        self.pop_history = []
        self.k_history = []

    def compute_growth(self):
        """Compute growth using the Lotka-Volterra model"""

        # Determine the limiting resource
        min_k = min(self.k_resources[res] for res in self.required_resources)

        # Avoid division by zero if resources are depleted
        if min_k == 0:
            if(self.population <= 2):
                return -1 * self.population # Kill population if small enough
            return self.growth_rate * -0.33 * self.population  # Extinction effect

        # Compute competition effect
        competition_effect = sum(self.competitors[m] for m in self.competitors)

        # Compute the growth
        growth = self.growth_rate * self.population * (1 - (competition_effect / min_k))

        # Prevent overshooting into negative population
        return max(growth, -self.population)
    
    def calculate_toxicity_multiplier(self, env_resources):
        """Calculate the impact of toxins on microbial growth"""

        # Find total resources 
        total_resources = sum(env_resources.values())

        if(total_resources == 0):
            return 0.0

        min_toxicity = 1.0

        for res in self.toxins:
            # Find weighted density of the current toxin
            cur_toxin = self.toxins[res]
            max_safe_density = cur_toxin["max_safe_density"]
            min_safe_density = cur_toxin["min_safe_density"]
            lethal_density = cur_toxin["lethal_density"]
            toxin_weighted_density = (cur_toxin["toxicity"] * env_resources[res]) / total_resources

            # If the density is in the safe range, continue to next toxin
            if (toxin_weighted_density <= max_safe_density and toxin_weighted_density >= min_safe_density):
                continue
            
            # If the density is lethal, return 0
            if(toxin_weighted_density >= lethal_density):
                return 0.0
            
            # If the density is between lethal and max safe density, then normalize the toxicity between the two
            if (toxin_weighted_density >= max_safe_density and toxin_weighted_density <= lethal_density):
                cur_toxicity = (toxin_weighted_density - lethal_density) / (max_safe_density - lethal_density)
            
            # If the density is less than the minimum that is safe, 
            if(toxin_weighted_density <= min_safe_density):
                cur_toxicity = (toxin_weighted_density) / (min_safe_density)
            
            # Keep track of the most toxic thing (least multiple because it acts as a multiplier)
            min_toxicity = min(min_toxicity, cur_toxicity)

        return min_toxicity

    def update_population(self, new_pop):
        """Update the population of a microbe and track the history"""

        self.pop_history.append(self.population)
        self.population = max(0, self.population + new_pop)

    def add_competitor(self, other_microbe):
        """Calculate the competition coefficient for another microbe based on shared resources"""

        # Reset competition coefficients to be empty
        competition_coefficients = []

        # Find the shared resources
        shared_resources = set(self.required_resources.keys()).intersection(set(other_microbe.required_resources.keys()))

        # Calculate competition coefficient
        for res in shared_resources:
            competition_coefficients.append(other_microbe.required_resources[res] / self.required_resources[res])

        # If no competition coefficients, the master competition coefficient is determined to be zero
        if not competition_coefficients:
            self.competitors[other_microbe] = 0

        # Otherwise, it's the max of the competition coefficients
        else:
            self.competitors[other_microbe] = other_microbe.population * max(competition_coefficients)
    
    def produce_consume_resources(self):
        """Calculate net resource change per time step"""

        # Dict to keep track of the resource changes
        resource_change = {}

        # Determine the limiting resource
        min_k = min(self.k_resources[res] for res in self.required_resources)

        # Add the consumed resources
        for res in self.required_resources:
            resource_change[res] = self.required_resources[res] * min_k * -1

        # Add the produced resources, adding to dict if necessary
        for res in self.produced_resources:
            if res in resource_change:
                resource_change[res] += self.produced_resources[res] * min_k
            else:
                resource_change[res] = self.produced_resources[res] * min_k

        return resource_change
    
    def compute_carry_capacity(self, env_resources):
        """Calculate the carrying capacity based on environmental resources and their toxicity"""

        # Find minimum toxicity multiplier
        toxicity_mult = self.calculate_toxicity_multiplier(env_resources)

        # Find the carry capacity
        for res in env_resources:
            resource_consumption = self.required_resources.get(res, 0)  # Per microbe consumption

            # If no pop, then k = 0
            if(self.population == 0):
                self.k_resources[res] = 0

            # Microbe doesn't use this resource, so no limit
            elif resource_consumption == 0:
                self.k_resources[res] = float('inf')

            # K = num resouces * toxicity multiplier / resource_consumption
            else:
                self.k_resources[res] = (env_resources[res] / resource_consumption) * toxicity_mult
        
        # Append the minimum to the history
        min_k = min(self.k_resources[res] for res in self.required_resources)
        self.k_history.append(min_k)

#
# --- ENVIRONMENT CLASS ---
#

class Environment:
    def __init__(self, initial_resources, resource_refresh_rate):
        """Initialize the environment with resources and refresh rates"""

        self.resources = initial_resources
        self.resource_refresh_rate = resource_refresh_rate
        self.resource_history = {res: [] for res in initial_resources}

    def update_resource_history(self):
        """Log and refresh resources over time"""

        # For all resources
        for res in self.resources:
            # Add current resource amount to history
            self.resource_history[res].append(self.resources[res])

            # Get resources from refresh rate
            self.resources[res] = max(0, self.resources[res] + self.resource_refresh_rate.get(res, 0))

    def add_resources(self, added_resources):
        """Add resources that come from outside sources (i.e. Microbes)"""

        for res in added_resources:
            self.resources[res] += added_resources[res]

#
# --- GRAPHING ---
#

# Smoothing on graphical representation
def moving_average(data, window_size):
    return np.convolve(data, np.ones(window_size)/window_size, mode='valid')

def graph_info(ax, window_size):
    for a in ax:
        a.clear()  # Clear previous plots

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
    plt.pause(0.1)

#
# -- SETUP ---
#

env = Environment(
    initial_resources={"Oxygen": 10, "Glucose": 0, "Lead": 0},
    resource_refresh_rate={"Oxygen": 10, "Glucose": 0, "Lead": 0}
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
        },),
]

#
# --- SIMULATION MENU ---
#

def get_user_input():
    while True:
        user_input = input("Press Enter to continue, 'menu' to see the menu, or type 'exit' to stop: ")

        if user_input.lower() == "menu":
            while True:  # Allows multiple menu interactions
                print("\n--- MENU ---")
                print("1) Add refreshing resource")
                print("2) Insert x amount of resource")
                print("3) Add a microbe")
                print("4) Edit existing microbe")
                print("5) Back to simulation")

                user_input = input("> ")

                match user_input:
                    case "1":  # Add refreshing resource
                        print("\nSelect a resource to increase refresh rate:")
                        resource_list = list(env.resource_refresh_rate.keys())

                        for index, resource in enumerate(resource_list):
                            print(f"{index}: {resource} (Current rate: {env.resource_refresh_rate[resource]})")

                        try:
                            choice = int(input("(Enter number)> "))
                            amount = int(input("Increase refresh rate by: "))

                            if 0 <= choice < len(resource_list):
                                resource_name = resource_list[choice]
                                env.resource_refresh_rate[resource_name] += amount
                                print(f"✅ {resource_name} refresh rate increased by {amount}")
                            else:
                                print("❌ Invalid selection.")

                        except ValueError:
                            print("❌ Please enter a valid number.")

                    case "2":  # Insert x amount of resource
                        print("\nSelect a resource to add:")
                        resource_list = list(env.resources.keys())

                        for index, resource in enumerate(resource_list):
                            print(f"{index}: {resource} (Current: {env.resources[resource]})")

                        try:
                            choice = int(input("(Enter number)> "))
                            amount = int(input("Amount to add: "))

                            if 0 <= choice < len(resource_list):
                                resource_name = resource_list[choice]
                                env.resources[resource_name] += amount
                                print(f"✅ Added {amount} {resource_name}.")
                            else:
                                print("❌ Invalid selection.")

                        except ValueError:
                            print("❌ Please enter a valid number.")

                    case "3":  # Add a microbe
                        print("\n--- ADD MICROBE ---")
                        name = input("Enter microbe name: ")
                        
                        try:
                            initial_population = int(input("Initial population: "))
                            growth_rate = float(input("Growth rate (e.g., 1.01): "))

                            required_resources = {}
                            produced_resources = {}
                            toxins = {}

                            # Get required resources
                            while True:
                                print("\nRequired resources:")
                                for idx, resource in enumerate(env.resources.keys()):
                                    print(f"{idx}: {resource}")

                                res_choice = input("Enter resource number to require (or 'done' to finish): ")
                                if res_choice.lower() == "done":
                                    break

                                try:
                                    res_idx = int(res_choice)
                                    res_name = list(env.resources.keys())[res_idx]
                                    res_amount = int(input(f"Amount of {res_name} required: "))
                                    required_resources[res_name] = res_amount
                                except (ValueError, IndexError):
                                    print("❌ Invalid selection.")

                            # Get produced resources
                            while True:
                                print("\nProduced resources:")
                                for idx, resource in enumerate(env.resources.keys()):
                                    print(f"{idx}: {resource}")

                                res_choice = input("Enter resource number to produce (or 'done' to finish): ")
                                if res_choice.lower() == "done":
                                    break

                                try:
                                    res_idx = int(res_choice)
                                    res_name = list(env.resources.keys())[res_idx]
                                    res_amount = int(input(f"Amount of {res_name} produced: "))
                                    produced_resources[res_name] = res_amount
                                except (ValueError, IndexError):
                                    print("❌ Invalid selection.")

                            # Get toxins
                            while True:
                                print("\nToxins:")
                                for idx, resource in enumerate(env.resources.keys()):
                                    print(f"{idx}: {resource}")

                                res_choice = input("Enter resource number to set as toxin (or 'done' to finish): ")
                                if res_choice.lower() == "done":
                                    break

                                try:
                                    res_idx = int(res_choice)
                                    res_name = list(env.resources.keys())[res_idx]
                                    
                                    toxicity = float(input(f"Toxicity of {res_name}: "))
                                    min_safe_density = float(input(f"Min safe density for {res_name}: "))
                                    max_safe_density = float(input(f"Max safe density for {res_name}: "))
                                    lethal_density = float(input(f"Lethal density for {res_name}: "))

                                    toxins[res_name] = {
                                        "toxicity": toxicity,
                                        "min_safe_density": min_safe_density,
                                        "max_safe_density": max_safe_density,
                                        "lethal_density": lethal_density
                                    }
                                except (ValueError, IndexError):
                                    print("❌ Invalid selection.")

                            # Ensure new microbes start at the correct time
                            new_microbe = Microbe(
                                name=name,
                                initial_population=initial_population,
                                growth_rate=growth_rate,
                                required_resources=required_resources,
                                produced_resources=produced_resources,
                                toxins=toxins
                            )
                            microbes.append(new_microbe)

                            # Fill history with NaN until the current time step
                            new_microbe.pop_history = [np.nan] * time + [initial_population]
                            new_microbe.k_history = [np.nan] * time + [0]  # Assuming 0 initial carrying capacity

                            # Immediately update the graph
                            graph_info(ax, window_size)

                        except ValueError:
                            print("❌ Invalid input, microbe creation aborted.")

                    case "4":  # Edit existing microbe
                        print("\n--- EDIT EXISTING MICROBE ---")
                        
                        # List microbes
                        if microbes:
                            print("Select a microbe to edit:")
                            for idx, microbe in enumerate(microbes):
                                print(f"{idx}: {microbe.name}")
                            try:
                                microbe_idx = int(input("Enter number to select microbe: "))
                                selected_microbe = microbes[microbe_idx]

                                # Edit population
                                print(f"Current population of {selected_microbe.name}: {selected_microbe.population}")
                                new_population = int(input("Enter new population (or leave blank to keep current): ") or selected_microbe.population)
                                selected_microbe.population = new_population

                                print(f"✅ Microbe '{selected_microbe.name}' updated!")

                            except (ValueError, IndexError):
                                print("❌ Invalid microbe selection.")

                        else:
                            print("❌ No microbes available to edit.")

                    case "5":  # Exit menu
                        break

                    case _:
                        print("❌ Invalid option. Try again.")

        elif user_input.lower() == "exit":
            return "exit"

        else:
            return None  # Continue simulation

#
# --- SIMULATION ---
#

# Set how long the simulation will run for
time_steps = 10000
window_size = 3
fig, ax = plt.subplots(1, 3, figsize=(18, 5))


# Begin the simulation
for time in range(time_steps):

    if(len(microbes) != 0):
        # Calculate competition coefficients at every time step because population is part of the calculations
        for m1 in microbes:
            m1.competitors = {}
            for m2 in microbes:
                m1.add_competitor(m2)

        # Process each microbe
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

    # Update graph
    graph_info(ax, window_size)

    # Wait for user input to proceed
    if(get_user_input() == "exit"):
        break

plt.close(fig)  # Close plot after simulation ends