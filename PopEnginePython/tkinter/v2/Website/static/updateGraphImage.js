var imageURI = 'static/images/plot.png';
var img = new Image();

// Function to update the canvas with the new image
function updateCanvas() {

    // Append an id for cache busting
    img.src = imageURI + '?d=' + Date.now();

    // Reference the canvas
    var canvas = document.getElementById("graphCanvas");
    var context = canvas.getContext("2d");

    // Clear and draw the image
    context.clearRect(0, 0, canvas.width, canvas.height);
    // Ensure image is fully loaded before drawing
    img.onload = function() {
        context.drawImage(img, 0, 0);
    };

    // Error handling in case the image fails to load
    img.onerror = function() {
        console.error("Error loading image.");
    };
}

document.getElementById("nextTimeStepButton").addEventListener("click", function() {
    // Show a loading indicator (optional)
    var loadingIndicator = document.getElementById("loadingIndicator");
    loadingIndicator.style.display = "block";

    // Send the AJAX request for next time step
    fetch('/nextTimeStep', {
        method: 'POST',
    })
    .then(response => {
        if (response.ok) {
            // After the simulation step, update the graph
            var imgElement = document.getElementById("graphCanvas");
            updateCanvas(imgElement);
        }
    })
    .catch(error => {
        console.error('Error:', error);
    })
    .finally(() => {
        // Hide the loading indicator
        loadingIndicator.style.display = "none";
    });
});

document.getElementById("ffButton").addEventListener("click", function() {
    // Get the fast forward amount
    var ffAmount = document.getElementById("ffAmount").value;

    // Show a loading indicator (optional)
    var loadingIndicator = document.getElementById("loadingIndicator");
    loadingIndicator.style.display = "block";

    // Send the AJAX request to fast forward
    fetch('/fastForward', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: `ffAmount=${ffAmount}`,  // Send the amount to the backend
    })
    .then(response => {
        if (response.ok) {
            // After fast forward, update the graph
            var imgElement = document.getElementById("graphCanvas");
            updateCanvas(imgElement);
        }
    })
    .catch(error => {
        console.error('Error:', error);
    })
    .finally(() => {
        // Hide the loading indicator
        loadingIndicator.style.display = "none";
    });
});

document.getElementById("resetButton").addEventListener("click", function() {
    // Send the AJAX request to fast forward
    fetch('/reset', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
    })
    .catch(error => {
        console.error('Error:', error);
    });
});

// Function to update the canvas with the new image
function updateCanvas(imgElement) {
    var img = new Image();
    img.src = 'static/images/plot.png' + '?d=' + Date.now();  // Cache busting

    var canvas = document.getElementById("graphCanvas");
    var context = canvas.getContext("2d");

    // Clear and draw the new image
    context.clearRect(0, 0, canvas.width, canvas.height);
    img.onload = function() {
        context.drawImage(img, 0, 0, 900, 250);
    };
}


