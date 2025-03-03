import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import numpy as np
import tkinter as tk
from tkinter import ttk, messagebox
import PIL.ImageTk

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
            # Add new resource
            if res not in self.resource_history:
                # Create entry
                self.resource_history[res] = []

                # Backlog history as 0
                for x in range(hist_len):
                    self.resource_history[res].append(0)

            hist_len = len(self.resource_history[res])

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

    canvas.draw()

#
# -- SETUP ---
#

env = Environment(
    initial_resources={"Oxygen": 10, "Glucose": 0, "Lead": 0},
    resource_refresh_rate={"Oxygen": 0, "Glucose": 0, "Lead": 0}
)

microbes = []

#
# --- SIMULATION ---
#

def advance_simulation():
    global current_step
    # Begin the simulation
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

    step_label.config(text=f"Time Step: {current_step}")
    current_step += 1

#
# --- GUI ---
#

class ResourceToxinWidget(ttk.Frame):
    """Widget containing multiple entry fields for a resource's toxins."""

    def __init__(self, parent, resource_name):
        super().__init__(parent)

        self.resource_name = resource_name
        self.toxin_var = tk.BooleanVar()

        # Checkbox to toggle the subfields
        self.checkbox = ttk.Checkbutton(self, text=resource_name, variable=self.toxin_var,
                                        command=self.toggle_fields)
        self.checkbox.pack(padx=5)

        # Frame for entry fields (initially hidden)
        self.fields_frame = ttk.Frame(self)
        
        # Creating multiple entry fields
        self.toxicity_field = ttk.Entry(self.fields_frame, width=5)
        self.min_safe_density_field = ttk.Entry(self.fields_frame, width=5)
        self.max_safe_density_field = ttk.Entry(self.fields_frame, width=5)
        self.lethal_density_field = ttk.Entry(self.fields_frame, width=5)

        ttk.Label(self.fields_frame, text="Toxicity:").pack(side="left", padx=2)
        self.toxicity_field.pack(side="left", padx=2)

        ttk.Label(self.fields_frame, text="Minimum Safe Density:").pack(side="left", padx=2)
        self.min_safe_density_field.pack(side="left", padx=2)

        ttk.Label(self.fields_frame, text="Maximum Safe Density:").pack(side="left", padx=2)
        self.max_safe_density_field.pack(side="left", padx=2)

        ttk.Label(self.fields_frame, text="Lethal Density:").pack(side="left", padx=2)
        self.lethal_density_field.pack(side="left", padx=2)

    def toggle_fields(self):
        """Show or hide the entry fields when the checkbox is toggled."""
        if self.toxin_var.get():
            self.fields_frame.pack(padx=5)
        else:
            self.fields_frame.pack_forget()

def display_dict(parent, data, prefix=""):
    for key, value in data.items():
        if isinstance(value, dict):
            display_dict(parent, value, prefix=f"{prefix}{key}.")
        else:
            label = ttk.Label(parent, text=f"{prefix}{key}: {value}", justify="left")
            label.pack(anchor="w", padx=10, pady=2)

def show_microbes_popup():
    popup = tk.Toplevel(root)
    popup.geometry("400x400")
    popup.title("Microbes list")

    # Create a canvas and scroll bar for scrollable content
    canvas = tk.Canvas(popup)
    scrollbar = ttk.Scrollbar(popup, orient="vertical", command=canvas.yview)
    scrollable_frame = ttk.Frame(canvas)

    # Configure the canvas
    canvas.configure(yscrollcommand=scrollbar.set)
    canvas.pack(side="left", fill="both", expand=True)
    scrollbar.pack(side="right", fill="y") 

    # Add scrollable frame to the canvas
    canvas.create_window((0, 0), window=scrollable_frame, anchor="nw")
    scrollable_frame.bind(
        "<Configure>",
        lambda e: canvas.configure(scrollregion=canvas.bbox("all"))
    )

    def update_microbes_GUI():
        for widget in scrollable_frame.winfo_children():
            widget.destroy()

        # Display no microbes
        if(len(microbes) == 0):
            microbe_label = ttk.Label(scrollable_frame, text="No microbes!", font=("Arial", 12, "bold"))
            microbe_label.pack(anchor="w", padx=10, pady=10)

        for microbe in microbes:
            # Create label for microbe name
            microbe_label = ttk.Label(scrollable_frame, text=f"Microbe: {microbe.name}", font=("Arial", 12, "bold"))
            microbe_label.pack(anchor="w", padx=10, pady=10)

            # Display population and growth rate
            ttk.Label(scrollable_frame, text=f"Population: {microbe.population}").pack(anchor="w", padx=10, pady=2)
            ttk.Label(scrollable_frame, text=f"Growth Rate: {microbe.growth_rate}").pack(anchor="w", padx=10, pady=2)

            # Display required resources
            ttk.Label(scrollable_frame, text="Required Resources:", font=("Arial", 10, "bold")).pack(anchor="w", padx=10, pady=5)
            display_dict(scrollable_frame, microbe.required_resources)

            # Display produced resources
            ttk.Label(scrollable_frame, text="Produced Resources:", font=("Arial", 10, "bold")).pack(anchor="w", padx=10, pady=5)
            display_dict(scrollable_frame, microbe.produced_resources)

            # Display toxins
            ttk.Label(scrollable_frame, text="Toxins:", font=("Arial", 10, "bold")).pack(anchor="w", padx=10, pady=5)
            display_dict(scrollable_frame, microbe.toxins)

            # Separator between microbes
            ttk.Separator(scrollable_frame, orient="horizontal").pack(fill="x", padx=10, pady=10)

        refresh_button = ttk.Button(scrollable_frame, text="Refresh", command=update_microbes_GUI)
        refresh_button.pack()
    
    update_microbes_GUI()

def add_microbe_popup():

    # Create new window
    form = tk.Toplevel(root)
    form.geometry("800x600")
    form.title("Add Microbe")

    # Create canvas, scroll frame, and scrollbar
    canvas = tk.Canvas(form)
    scrollbar = ttk.Scrollbar(form, orient="vertical", command=canvas.yview)
    scroll_frame = ttk.Frame(canvas)

    # Set the canvas and scrollbar so they will coexist
    canvas.configure(yscrollcommand=scrollbar.set)
    canvas.pack(fill="both", side="left", expand=True)
    scrollbar.pack(side="right", fill="y")

    # Get the id of the canvas window
    window_id = canvas.create_window((canvas.winfo_width() / 2, 0), window=scroll_frame, anchor="n")

    # Adjust centering when the canvas is resized
    def update_scroll_window(event):
        canvas.itemconfigure(window_id, width=canvas.winfo_width() - scrollbar.winfo_width())
        canvas.coords(window_id, canvas.winfo_width() / 2, 0)

    # Update window on resize
    canvas.bind("<Configure>", update_scroll_window)

    # Handle scroll
    scroll_frame.bind(
        "<Configure>",
        lambda e: canvas.configure(scrollregion=canvas.bbox("all"))
    )

    # Form submission
    def submit_microbe():
        name = name_entry.get()
        population = initial_population_entry.get()
        growth_rate = growth_rate_entry.get()
        required_resources = {}
        produced_resources = {}
        toxins = {}

        # Turn the checkboxes and vars into valid dict entries
        for resource, var in required_resource_vars.items():
            if(var.get()):
                required_resources[resource] = int(req_quantity_entry[resource].get().strip())
        
        for resource, var in produced_resource_vars.items():
            if(var.get()):
                produced_resources[resource] = int(prod_quantity_entry[resource].get().strip())
        
        for toxin in toxin_widgets:
            if(toxin.toxin_var.get()):
                resource = toxin.resource_name
                toxins[resource] = {}
                toxins[resource]["toxicity"] = float(toxin.toxicity_field.get().strip())
                toxins[resource]["min_safe_density"] = float(toxin.min_safe_density_field.get().strip())
                toxins[resource]["max_safe_density"] = float(toxin.max_safe_density_field.get().strip())
                toxins[resource]["lethal_density"] = float(toxin.lethal_density_field.get().strip())


        new_microbe = Microbe (
            name=name,
            initial_population=float(population),
            growth_rate=float(growth_rate),
            required_resources=required_resources,
            produced_resources=produced_resources,
            toxins=toxins
        )

        for x in range(current_step):
            new_microbe.pop_history.append(0)
            new_microbe.k_history.append(0)

        microbes.append(new_microbe)
        form.destroy()
    # Get the name
    ttk.Label(scroll_frame, text="Name:").pack(pady=5)
    name_entry = ttk.Entry(scroll_frame)
    name_entry.pack(pady=5)

    # Get the initial population
    ttk.Label(scroll_frame, text="Initial Population:").pack(pady=5)
    initial_population_entry = ttk.Entry(scroll_frame)
    initial_population_entry.pack(pady=5)

    # Get Growth Rate
    ttk.Label(scroll_frame, text="Growth Rate:").pack(pady=5)
    growth_rate_entry = ttk.Entry(scroll_frame)
    growth_rate_entry.pack(pady=5)

    # Get Required Resources (with quantities)
    ttk.Label(scroll_frame, text="Required Resources:").pack(pady=5)

    required_resource_vars = {}
    req_quantity_entry = {}
    
    for resource in env.resources:
        req_var = tk.BooleanVar()
        required_resource_vars[resource] = req_var
        
        # Create a frame for the checkbox and quantity
        resource_frame = ttk.Frame(scroll_frame)
        resource_frame.pack(pady=2, anchor='center')

        # Checkbox for resource
        checkbox = ttk.Checkbutton(resource_frame, text=resource, variable=req_var)
        checkbox.pack(side='left', padx=5)

        # Entry for quantity
        quantity_entry_field = ttk.Entry(resource_frame, width=5)
        quantity_entry_field.pack(side='left', padx=5)
        
        req_quantity_entry[resource] = quantity_entry_field

    # Get Produced Resources (with quantities)
    ttk.Label(scroll_frame, text="Produced Resources:").pack(pady=5)
    
    produced_resource_vars = {}
    prod_quantity_entry = {}
    
    for resource in env.resources:
        prod_var = tk.BooleanVar()
        produced_resource_vars[resource] = prod_var
        
        # Create a frame for the checkbox and quantity
        resource_frame = ttk.Frame(scroll_frame)
        resource_frame.pack(pady=2, anchor='center')

        # Checkbox for resource
        checkbox = ttk.Checkbutton(resource_frame, text=resource, variable=prod_var)
        checkbox.pack(side='left', padx=5)

        # Entry for quantity
        quantity_entry_field = ttk.Entry(resource_frame, width=5)
        quantity_entry_field.pack(side='left', padx=5)
        
        prod_quantity_entry[resource] = quantity_entry_field

    # Get Produced Resources (with quantities)
    ttk.Label(scroll_frame, text="Toxins:").pack(pady=5)
    
    toxin_widgets = []
    for resource in env.resources:
        toxin = ResourceToxinWidget(scroll_frame, resource)
        toxin.pack(pady=2, anchor='center', fill='x')
        toxin_widgets.append(toxin)
    
    # Create Microbe button
    submit_button = tk.Button(scroll_frame, text="Create Microbe", command=submit_microbe)
    submit_button.pack(pady=5)

def remove_microbe_popup():
    # Create the window
    popup = tk.Toplevel(root)
    popup.geometry("400x600")
    popup.title("Remove Microbes")

    # Create the canvas and scrollbar
    canvas = tk.Canvas(popup)
    scrollbar = ttk.Scrollbar(popup, orient="vertical", command=canvas.yview)
    scrollable_frame = ttk.Frame(canvas)

    # Configure the canvas
    canvas.configure(yscrollcommand=scrollbar.set)
    canvas.pack(side="left", fill="both", expand=True)
    scrollbar.pack(side="right",fill="y")

    # Add a scrollable frame to the canvas
    canvas.create_window((0, 0), window=scrollable_frame, anchor="nw")
    scrollable_frame.bind(
        "<Configure>",
        lambda e: canvas.configure(scrollregion=canvas.bbox("all"))
    )

    microbes_to_remove_vars = {}

    def remove_microbes():
        # Append microbes to remove to a list to avoid modifying as we iterate
        remove_list = []
        for microbe in microbes:
            if (microbes_to_remove_vars[microbe].get()):
                remove_list.append(microbe)

        # Remove all the ones in the list
        for microbe in remove_list:
            microbes.remove(microbe)

        # Update the GUI
        update_remove_microbe_gui()

    def update_remove_microbe_gui():
        # Access the right dict
        nonlocal microbes_to_remove_vars

        # Destroy all the existing widgets
        for widget in scrollable_frame.winfo_children():
            widget.destroy()

        for microbe in microbes:
            # Set up our value to access later
            var = tk.BooleanVar()
            microbes_to_remove_vars[microbe] = var

            # Checkbox
            checkbox = ttk.Checkbutton(scrollable_frame, text=microbe.name, variable=var)
            checkbox.pack(padx=5, pady=5)

        # Button to remove
        remove_button = tk.Button(scrollable_frame, text="Remove selected microbes", command=remove_microbes)
        remove_button.pack(padx=5, pady=5)

    # Update the gui on open
    update_remove_microbe_gui()


def view_environment_popup():
    # Create window
    popup = tk.Toplevel(root)
    popup.geometry("400x600")
    popup.title("Environment Info")

    # Create canvas and scrollbar
    canvas = tk.Canvas(popup)
    scrollbar = ttk.Scrollbar(popup, orient="vertical", command=canvas.yview)
    scrollable_frame = ttk.Frame(canvas)

    # Configure the canvas
    canvas.configure(yscrollcommand=scrollbar.set)
    canvas.pack(side="left", fill="both", expand=True)
    scrollbar.pack(side="right", fill="y")

    # Add scrollable frame to canvas
    canvas.create_window((0, 0), window=scrollable_frame, anchor="nw")
    scrollable_frame.bind(
        "<Configure>",
        lambda e: canvas.configure(scrollregion=canvas.bbox("all"))
    )

    def update_env_view():
        for widget in scrollable_frame.winfo_children():
            widget.destroy()

        # Handle no resources
        if(len(env.resources) == 0):
            no_resources_label = ttk.Label(scrollable_frame, text="No resources!", font=("Arial", 12, "bold"))
            no_resources_label.pack(anchor="w", padx=10, pady=10)
            return

        # For each resource, print:
        for resource in env.resources:
            # Name
            resource_label = ttk.Label(scrollable_frame, text=f"Resource: {resource}", font=("Arial", 12, "bold"))
            resource_label.pack(anchor="w", padx=10, pady=10)

            # Current amount
            amount_label = ttk.Label(scrollable_frame, text=f"Amount: {env.resources[resource]}", font=("Arial", 12))
            amount_label.pack(anchor="w", padx=10, pady=10)

            # Refresh rate
            refresh_label = ttk.Label(scrollable_frame, text=f"Refresh amount: {env.resource_refresh_rate[resource]}", font=("Arial", 12))
            refresh_label.pack(anchor="w", padx=10, pady=10)

            # Separator
            ttk.Separator(scrollable_frame, orient="horizontal").pack(fill="x", padx=10, pady=10)

        update_env_view_button = ttk.Button(scrollable_frame, text="Update info", command=update_env_view)
        update_env_view_button.pack()

    update_env_view()


def edit_environment_popup():
    # Create window
    popup = tk.Toplevel(root)
    popup.geometry("400x600")
    popup.title("Edit Environment")

    # Create canvas and scrollbar
    canvas = tk.Canvas(popup)
    scrollbar = ttk.Scrollbar(popup, orient="vertical", command=canvas.yview)
    scrollable_frame = ttk.Frame(canvas)

    # configure the canvas
    canvas.configure(yscrollcommand=scrollbar.set)
    canvas.pack(side="left", fill="both", expand=True)
    scrollbar.pack(side="right", fill="y")

    # add scrollable frame to canvas
    canvas.create_window((0, 0), window=scrollable_frame, anchor="nw")
    scrollable_frame.bind(
        "<Configure>",
        lambda e: canvas.configure(scrollregion=canvas.bbox("all"))
    )

    def update_env_resources_GUI():
        for resource in env.resources:
            env.resources[resource] = int(float(amount_entries[resource].get().strip()))
            env.resource_refresh_rate[resource] = int(float(refresh_entries[resource].get().strip()))
        popup.destroy()

    amount_entries = {}
    refresh_entries = {}

    for resource in env.resources:
        # Create frame
        resource_frame = ttk.Frame(scrollable_frame)
        resource_frame.pack(pady=2)

        # Resource label
        resource_label = ttk.Label(resource_frame, text=f"Resource: {resource}", font=("Arial", 12, "bold"))
        resource_label.grid(row=0, column=0, columnspan=2, sticky="w", padx=10, pady=10)

        # Resource amount
        amount_label = ttk.Label(resource_frame, text="Current amount: ", font=("Arial", 12))
        amount_label.grid(row=1, column=0, sticky="w", padx=10, pady=5)

        # Amount entry
        amount_entry_field = ttk.Entry(resource_frame, width = 5)
        amount_entry_field.insert(0, env.resources[resource])
        amount_entry_field.grid(row=1, column=1, padx=5, pady=5)
        
        amount_entries[resource] = amount_entry_field

        # Refresh amount
        refresh_label = ttk.Label(resource_frame, text="Current refresh amount: ", font=("Arial", 12))
        refresh_label.grid(row=2, column=0, sticky="w", padx=10, pady=5)

        # Refresh entry
        refresh_entry_field = ttk.Entry(resource_frame, width = 5)
        refresh_entry_field.insert(0, env.resource_refresh_rate[resource])
        refresh_entry_field.grid(row=2, column=1, padx=5, pady=5)

        refresh_entries[resource] = refresh_entry_field
    
    submit_button = ttk.Button(scrollable_frame, text="Submit", command=update_env_resources_GUI)
    submit_button.pack()

def add_resources_popup():
    # Create the window
    popup = tk.Toplevel(root)
    popup.geometry("175x100")
    popup.title("Remove Microbes")

    # Create the canvas and scrollbar
    canvas = tk.Canvas(popup)
    scrollbar = ttk.Scrollbar(popup, orient="vertical", command=canvas.yview)
    scrollable_frame = ttk.Frame(canvas)

    # Configure the canvas
    canvas.configure(yscrollcommand=scrollbar.set)
    canvas.pack(side="left", fill="both", expand=True)
    scrollbar.pack(side="right",fill="y")

    # Add a scrollable frame to the canvas
    canvas.create_window((0, 0), window=scrollable_frame, anchor="nw")
    scrollable_frame.bind(
        "<Configure>",
        lambda e: canvas.configure(scrollregion=canvas.bbox("all"))
    )

    def submit_add_resource():
        new_microbe_name = new_microbe_entry.get()
        env.resources[new_microbe_name] = 0
        env.resource_refresh_rate[new_microbe_name] = 0
        popup.destroy()

    new_microbe_label = tk.Label(scrollable_frame, text="Name of new resource")
    new_microbe_label.pack(padx=5, pady=5)

    new_microbe_entry = ttk.Entry(scrollable_frame)
    new_microbe_entry.pack(padx=5, pady=5)

    submit_button = tk.Button(scrollable_frame, text="Submit", command=submit_add_resource)
    submit_button.pack(padx=5, pady=5)

def fast_forward_pressed():
    for x in range(ff_amount):
        advance_simulation()
    graph_info(ax, window_size)

def next_time_step_pressed():
    advance_simulation()
    graph_info(ax, window_size)

def quit_pressed():
    plt.close(fig)
    root.destroy()

def on_ff_amount_change(event=None):
    global ff_amount
    try:
        ff_amount = int(ff_amount_spinbox.get())
    except ValueError:
        print("Invalid")

def microbes_button_pressed():
    # Create window
    form = tk.Toplevel(root)
    form.geometry("400x300")
    form.title("Microbe Options")

    # Add microbes
    add_microbe_button = tk.Button(form, text="Add microbes", command=add_microbe_popup)
    add_microbe_button.pack()

    # Remove microbes
    remove_microbes_button = tk.Button(form, text="Remove Microbes", command=remove_microbe_popup)
    remove_microbes_button.pack()

    # View microbes
    show_microbes_button = tk.Button(form, text="Show microbes", command=show_microbes_popup)
    show_microbes_button.pack()

def environment_button_pressed():
    form = tk.Toplevel(root)
    form.geometry("400x300")
    form.title("Environment Options")

    # View environment stats
    view_environment_button = tk.Button(form, text="View environment", command=view_environment_popup)
    view_environment_button.pack()

    # Edit environment resources
    edit_environment_button = tk.Button(form, text="Edit environment", command=edit_environment_popup)
    edit_environment_button.pack()

    # Add new resource
    add_resource_button = tk.Button(form, text="Add Resource", command=add_resources_popup)
    add_resource_button.pack()

# Set how long the simulation will run for
window_size = 3
current_step = 0
ff_amount = 1

# TKinter window
root = tk.Tk()
root.title("Microbe Simulation")

fig, ax = plt.subplots(1, 3, figsize=(18, 5))
canvas = FigureCanvasTkAgg(fig, master=root)
canvas.get_tk_widget().pack()

# Labels to show timestep
step_label = tk.Label(root, text=f"Time Step: {current_step}")
step_label.pack()

# Button to advance time
next_button = tk.Button(root, text="Next Step", command=next_time_step_pressed)
next_button.pack()

# Fast forward x time
ff_button = tk.Button(root, text="Fast Forward", command=fast_forward_pressed)
ff_button.pack()

# spinbox to choose how much to fast forward
ff_amount_spinbox = tk.Spinbox(root, from_=1, to=100, command=on_ff_amount_change)
ff_amount_spinbox.pack()
ff_amount_spinbox.bind("<KeyRelease>", on_ff_amount_change)

# Microbes
microbe_button = tk.Button(root, text="Microbe Options", command=microbes_button_pressed)
microbe_button.pack()

# Environment
environment_button = tk.Button(root, text="Environment Options", command=environment_button_pressed)
environment_button.pack()

# Quit button
quit_button = tk.Button(root, text="Quit", command=quit_pressed)
quit_button.pack()

root.mainloop()