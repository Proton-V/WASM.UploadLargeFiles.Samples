const makeRequest = (method, url, formData, onprogress) => {
    const xhr = new XMLHttpRequest();
    return new Promise(resolve => {
        xhr.open(method, url, true);
        xhr.onload = () => resolve({
            status: xhr.status,
            response: xhr.responseText
        });
        xhr.onerror = () => resolve({
            status: xhr.status,
            response: xhr.responseText
        });
        xhr.upload.onprogress = onprogress
        xhr.send(formData);
    })
}

async function uploadFiles(dotnetObj, onprogressFunc, inputFieldId, url) {
    var formData = new FormData();
    var files = document.getElementById(inputFieldId).files;
    for (i = 0; i < files.length; i++) {
        formData.append("file"+i, files[i]);
    }
    var onprogress = event => {
        dotnetObj.invokeMethodAsync(onprogressFunc, event.loaded, event.total);
    }
    var result = await makeRequest("POST", url, formData, onprogress);

    alert("Status:" + result.status);
}