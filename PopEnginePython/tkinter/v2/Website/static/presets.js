document.getElementById("basicSymbiosis").addEventListener("click", function() {
    // Send the AJAX request for next time step
    fetch('/basic_symbiosis', {
        method: 'POST',
    })
    .catch(error => {
        console.error('Error:', error);
    });
});

document.getElementById("basicWithLead").addEventListener("click", function() {
    // Send the AJAX request for next time step
    fetch('/basic_with_lead', {
        method: 'POST',
    })
    .catch(error => {
        console.error('Error:', error);
    });
});

document.getElementById("3MicrobeSymbiosis").addEventListener("click", function() {
    // Send the AJAX request for next time step
    fetch('/3_microbe_symbiosis', {
        method: 'POST',
    })
    .catch(error => {
        console.error('Error:', error);
    });
});