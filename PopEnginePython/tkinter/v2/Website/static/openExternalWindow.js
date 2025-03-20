function openNewWindow(path) {
    window.open(path, 'EnvWindow', 'width=400,height=600')
}

function closeWindow() {
    setTimeout(() => {
        window.close();
    }, 100); // Delay of 1000 milliseconds (1 second)
}


function reloadWindow() {
    window.location.reload();
}