document.getElementById('uploadButton').addEventListener('click', async () => {
    const fileInput = document.getElementById('fileInput');
    const file = fileInput.files[0];
    if (!file) {
        alert('Select a file to upload.');
        return;
    }

    const host = 'http://localhost:5239/api/v1';
    const chunkSize = 1024 * 1024 * 2; // 2MB chunk size; sync with API
    const totalChunks = Math.ceil(file.size / chunkSize);
    const progressBar = document.getElementById('uploadProgress');
    const blobId = generateGuid();
    let chunksNumbers = [];

    for (let i = 0; i < totalChunks; i++) {
        const start = i * chunkSize;
        const end = Math.min(start + chunkSize, file.size);
        const chunk = file.slice(start, end);
        progressBar.value = (i / totalChunks) * 100;

        try {
            const response = await fetch(`${host}/files/chunk`, {
                method: 'POST',
                headers: {
                    'ChunkNumber': i + 1,
                    'BlobId': blobId
                },
                body: chunk
            });

            if (!response.ok) {
                throw new Error('Upload failed');
            }

            chunkNumbers.push(i + 1);
        } catch (error) {
            console.error('Error uploading chunk:', error);
            alert('Upload failed. See console for details.');
            return;
        }
    }

    // Commit the upload
    try {
        const commitResponse = await fetch(`${host}/files/commit`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                BlobId: blobId,
                ContentType: file.type,
                ChunksNumbers: chunksNumbers
            })
        });

        if (!commitResponse.ok) {
            throw new Error('Commit failed');
        }

        progressBar.value = 100;
        alert('Upload completed');
    } catch (error) {
        console.error('Error committing upload:', error);
        alert('Commit failed');
    }
});

function generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        const r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}