import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
import numpy as np
import tkinter as tk
from tkinter import ttk
from flask import Flask, render_template, request, redirect, url_for
import logging
import os

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
            resource_change[res] = self.required_resources[res] * min(min_k, self.population) * -1

        # Add the produced resources, adding to dict if necessary
        for res in self.produced_resources:
            if res in resource_change:
                resource_change[res] += self.produced_resources[res] * min(min_k, self.population)
            else:
                resource_change[res] = self.produced_resources[res] * min(min_k, self.population)

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
            self.resources[res] += self.resource_refresh_rate[res]
            if(self.resources[res] < 0):
                self.resources[res] = 0

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

#
# -- SETUP ---
#

env = Environment(
    initial_resources={"Oxygen": 2, "Glucose": 2, "Lead": 1},
    resource_refresh_rate={"Oxygen": 0, "Glucose": 0, "Lead": 1}
)

microbes = [
    Microbe(
        name="O2Eater",
        initial_population=1,
        growth_rate=1.2,
        required_resources={"Oxygen": 1},
        produced_resources={"Glucose": 1},
        toxins={
            "Lead": {"toxicity": 1.0, "min_safe_density": 0.0, "max_safe_density": 0.4, "lethal_density": 0.6}
        }
    ),
    Microbe(
        name="GlucoseEater",
        initial_population=1,
        growth_rate=1.2,
        required_resources={"Glucose": 1},
        produced_resources={"Oxygen": 1},
        toxins={}
    ),
    Microbe(
        name="LeadEater",
        initial_population=1,
        growth_rate=1.2,
        required_resources={"Lead": 1},
        produced_resources={},
        toxins={}
    ),
]

#
# --- SIMULATION ---
#

def advance_simulation():
    global current_step

    if(len(env.resources) == 0):
        return

    resCounter = 0
    for res in env.resources:
        if env.resources[res] > 0:
            resCounter += 1
    
    if(resCounter == 0):
        return

    # Begin the simulation
    if(len(microbes) != 0):
        # Calculate competition coefficients at every time step because population is part of the calculations
        for m1 in microbes:
            m1.competitors = {}
            for m2 in microbes:
                m1.add_competitor(m2)

        total_resource_usage = {}

        # Process each microbe
        for microbe in microbes:
            # Get carry capacity of microbe
            microbe.compute_carry_capacity(env.resources)

            # Get resource changes due to microbe
            net_resource_usage = microbe.produce_consume_resources()
            for resource in net_resource_usage:
                if resource in total_resource_usage:
                    total_resource_usage[resource] += net_resource_usage[resource]
                else:
                    total_resource_usage[resource] = net_resource_usage[resource]

            # Calculate new microbe population
            pop_change = microbe.compute_growth()
            microbe.update_population(pop_change)

    # Log resource history
    env.add_resources(total_resource_usage)
    env.update_resource_history()

    current_step += 1

#
# --- GRAPHING ---
#

# Create the plot
fig, ax = plt.subplots(1, 3, figsize=(18, 5))
current_step = 0

def reset_graph():
    global current_step
    current_step = 0
    plt.cla()

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
    
    plt.savefig('./static/images/plot.png')

#
# --- WEBSITE ---
#

app = Flask(__name__)

@app.route("/")
def init():
    advance_simulation()
    graph_info(ax, 3)
    return render_template("index.html")

@app.route("/nextTimeStep", methods=['POST'])
def next_time_step():
    if(len(env.resouces > 0)):
        advance_simulation()
        graph_info(ax, 3)
    return '', 204

@app.route("/fastForward", methods=["POST"])
def fast_forward():
    if(len(env.resources) <= 0):
        return '', 204

    ff_amount = request.form.get("ffAmount")
    for x in range(int(ff_amount)):
        advance_simulation()
    print("Saving plot")
    graph_info(ax, 3)
    print("Returning")
    return '', 204

@app.route("/envOptions", methods=["POST", "GET"])
def env_options():
    return render_template("env_opt.html")

@app.route("/add_resource", methods=["POST", "GET"])
def add_resource():
    # Handle visible web page
    if(request.method == 'GET'):
        return render_template("add_resource.html")
    
    # Add the resource
    new_resource_name = request.form.get("resourceName")

    # Handle empty
    if(new_resource_name == ""):
        return '', 204
    
    env.resources[new_resource_name] = 0
    env.resource_refresh_rate[new_resource_name] = 0

    # Return so flask doesnt get mad
    return '', 204

@app.route("/edit_env", methods=["POST", "GET"])
def edit_env():
    # Handle visible web page
    if(request.method == 'GET'):
        return render_template("edit_env.html", resources=env.resources, refresh=env.resource_refresh_rate)

    # Handle backend POST
    for res in env.resources:
        # Get the keys
        amount_key = f"{res}_amount"
        rate_key = f"{res}_rate"

        # Get vals from keys
        amount = float(request.form.get(amount_key))
        rate = float(request.form.get(rate_key))
        
        # Insert vals into dicts
        if(amount is not None and rate is not None):
            env.resources[res] = amount
            env.resource_refresh_rate[res] = rate

    return '', 204

@app.route("/microbeOptions", methods=["POST", "GET"])
def microbe_options():
    return render_template("microbe_opt.html")

@app.route("/create_microbe", methods=["POST", "GET"])
def create_microbe():
    if(request.method == 'GET'):
        return render_template("create_microbe.html", resources=env.resources)
    
    # Handle POST
    new_microbe_name = request.form.get("microbe_name")
    new_microbe_pop = float(request.form.get("population"))
    new_microbe_growth = float(request.form.get("growth_rate"))

    req_resources = {}
    prod_resources = {}
    toxins = {}

    # Get REquired resources
    for res in env.resources:
        req_res_key = f"{res}_required_amount"
        req_res_amt = float(request.form.get(req_res_key))

        # Throw away bad entries
        if(req_res_amt <= 0):
            continue
        
        # Add it to the dict
        req_resources[res] = req_res_amt

    # Get produced resources
    for res in env.resources:
        prod_res_key = f"{res}_produced_amount"
        prod_res_amt = float(request.form.get(prod_res_key))

        # Throw away bad entries
        if(prod_res_amt <= 0):
            continue
        
        # Add it to the dict
        prod_resources[res] = prod_res_amt

    for res in env.resources:
        toxicity_key = f"{res}_toxicity"
        min_safe_toxicity_key = f"{res}_min_safe_toxicity"
        max_safe_toxicity_key = f"{res}_max_safe_toxicity"
        lethal_toxicity_key = f"{res}_lethal_toxicity"

        res_toxicity = request.form.get(toxicity_key)

        # Throw away bad responses
        if(res_toxicity == ''):
            continue
        else:
            res_toxicity = float(res_toxicity)

        # Append to dictionary
        toxins[res] = {}
        toxins[res]["toxicity"] = float(request.form.get(toxicity_key))
        toxins[res]["min_safe_density"] = float(request.form.get(min_safe_toxicity_key))
        toxins[res]["max_safe_density"] = float(request.form.get(max_safe_toxicity_key))
        toxins[res]["lethal_density"] = float(request.form.get(lethal_toxicity_key))

    # Create the new microbe
    new_microbe = Microbe(
        name = new_microbe_name,
        initial_population = new_microbe_pop,
        growth_rate = new_microbe_growth,
        required_resources = req_resources,
        produced_resources = prod_resources,
        toxins = toxins
    )

    # Append it
    microbes.append(new_microbe)

    return '', 204

@app.route("/edit_microbes", methods=['GET', 'POST'])
def edit_microbes():
    if(request.method == 'GET'):
        return render_template("edit_microbes.html", microbes=microbes)
    
    # Handle POST
    for microbe in microbes:
        pop_key = f"{microbe}_population"

        pop_amt = request.form.get(pop_key)
        if(pop_amt == ''):
            continue

        microbe.population = float(pop_amt)
    
    return '', 204

@app.route("/delete_microbes", methods=['GET', 'POST'])
def delete_microbes():
    if(request.method == 'GET'):
        return render_template("delete_microbes.html", microbes=microbes)
    
    delete_list = []
    
    # handle POST
    for microbe in microbes:
        checkbox_key = f"{microbe}_checkbox"
        checkbox_value = request.form.get(checkbox_key)

        if(checkbox_value == 'on'):
            delete_list.append(microbe)

    for microbe in delete_list:
        microbes.remove(microbe)

    return '', 204

@app.route("/presets", methods=['GET', 'POST'])
def presets():
    if(request.method == 'GET'):
        return render_template("presets.html")
    
@app.route("/reset", methods=['POST'])
def reset():
    global microbes
    global env

    microbes = []
    env = Environment(
        initial_resources={},
        resource_refresh_rate={}
    )

    reset_graph()
    graph_info(ax, 3)
    return '', 204
    
@app.route("/basic_symbiosis", methods=['POST'])
def basic_symbiosis():
    global env
    global microbes

    reset_graph()

    env = Environment(
        initial_resources={"Oxygen": 10, "Glucose": 10, "Lead": 0},
        resource_refresh_rate={"Oxygen": 0, "Glucose": 0, "Lead": 0}
    )

    microbes = [
        Microbe(
            name="OxygenEater",
            initial_population=1,
            growth_rate=1.2,
            required_resources={"Oxygen": 1},
            produced_resources={"Glucose": 1},
            toxins={
                "Lead": {"toxicity": 1.0, "min_safe_density": 0.0, "max_safe_density": 0.4, "lethal_density": 0.6}
            }
        ),
        Microbe(
            name="GlucoseEater",
            initial_population=1,
            growth_rate=1.2,
            required_resources={"Glucose": 1},
            produced_resources={"Oxygen": 1},
            toxins={}
        ),
    ]

    advance_simulation()

@app.route("/basic_with_lead", methods=['POST'])
def basic_with_lead():
    global env
    global microbes

    reset_graph()

    env = Environment(
        initial_resources={"Oxygen": 10, "Glucose": 10, "Lead": 0},
        resource_refresh_rate={"Oxygen": 0, "Glucose": 0, "Lead": 1}
    )

    microbes = [
        Microbe(
            name="OxygenEater",
            initial_population=1,
            growth_rate=1.2,
            required_resources={"Oxygen": 1},
            produced_resources={"Glucose": 1},
            toxins={
                "Lead": {"toxicity": 1.0, "min_safe_density": 0.0, "max_safe_density": 0.4, "lethal_density": 0.6}
            }
        ),
        Microbe(
            name="GlucoseEater",
            initial_population=1,
            growth_rate=1.2,
            required_resources={"Glucose": 1},
            produced_resources={"Oxygen": 1},
            toxins={}
        ),
    ]

    advance_simulation()

@app.route("/3_microbe_symbiosis", methods=['POST'])
def three_microbe_symbiosis():
    global env
    global microbes

    reset_graph()

    env = Environment(
        initial_resources={"Oxygen": 10, "Glucose": 10, "Lead": 1},
        resource_refresh_rate={"Oxygen": 0, "Glucose": 0, "Lead": 1}
    )

    microbes = [
        Microbe(
            name="O2Eater",
            initial_population=1,
            growth_rate=1.2,
            required_resources={"Oxygen": 1},
            produced_resources={"Glucose": 1},
            toxins={
                "Lead": {"toxicity": 1.0, "min_safe_density": 0.0, "max_safe_density": 0.4, "lethal_density": 0.6}
            }
        ),
        Microbe(
            name="GlucoseEater",
            initial_population=1,
            growth_rate=1.2,
            required_resources={"Glucose": 1},
            produced_resources={"Oxygen": 1},
            toxins={}
        ),
        Microbe(
            name="LeadEater",
            initial_population=1,
            growth_rate=1.2,
            required_resources={"Lead": 1},
            produced_resources={},
            toxins={}
        ),
    ]

    advance_simulation()