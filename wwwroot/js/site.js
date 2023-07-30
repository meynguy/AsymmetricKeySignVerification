// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Function to generate a key pair
async function generateSigningKeyPair() {
    const keyPair = await window.crypto.subtle.generateKey(
        {
            name: "ECDSA",
            namedCurve: "P-384",
        },
        true,
        ["sign", "verify"],
    );

    return keyPair;
}

// Function to export the public key
async function exportPublicKey(keyPair) {
    const publicKey = await crypto.subtle.exportKey('jwk', keyPair.publicKey);
    return publicKey;
}

// Function to export the private key
async function exportPrivateKey(keyPair) {
    const privateKey = await crypto.subtle.exportKey('jwk', keyPair.privateKey);
    return privateKey;
}
// Function to import the public key
async function importPublicKey(publicKeyData) {
    const publicKey = await crypto.subtle.importKey(
        "jwk",
        publicKeyData,
        {
            name: "ECDSA",
            namedCurve: "P-384",
        },
        true,
        ["verify"]
    );

    return publicKey;
}

// Function to import the private key
async function importPrivateKey(privateKeyData) {
    const privateKey = await crypto.subtle.importKey(
        "jwk",
        privateKeyData,
        {
            name: "ECDSA",
            namedCurve: "P-384",
        },
        true,
        ["sign"]
    );

    return privateKey;
}

// Function to sign a message
async function signMessage(message, privateKey) {
    const encodedMessage = new TextEncoder().encode(message);
    const signature = await crypto.subtle.sign(
        {
            name: "ECDSA",
            hash: { name: "SHA-256" },
        },
        privateKey,
        encodedMessage
    );
    return new Uint8Array(signature);
}

// Function to verify a signature
async function verifySignature(message, signature, publicKey) {
    const encodedMessage = new TextEncoder().encode(message);
    const isSignatureValid = await crypto.subtle.verify(
        {
            name: "ECDSA",
            hash: { name: "SHA-256" },
        },
        publicKey,
        signature,
        encodedMessage
    );
    return isSignatureValid;
}


async function main() {
    var keyPair = await generateSigningKeyPair();

    var publicKeyData = await exportPublicKey(keyPair);


    var privateKeyData = await exportPrivateKey(keyPair);
    localStorage.setItem("SV-privateKey", JSON.stringify(privateKeyData));

    const message = 'Hello, SubtleCrypto!';

    // Retrieve the private key from storage and import it
    const storedPrivateKeyData = JSON.parse(localStorage.getItem("SV-privateKey"));

    var importedPrivateKey = await importPrivateKey(storedPrivateKeyData);

    var signature = await signMessage(message, importedPrivateKey);

    var importedPublicKey = await importPublicKey(publicKeyData);

    if (await verifySignature(message, signature, importedPublicKey)) {

        fetch('verifySignature', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                messageAsBase64: btoa(message),
                signatureAsBase64: btoa(String.fromCharCode.apply(null, signature)),
                publicKeyAsBase64: btoa(JSON.stringify(publicKeyData))
            })
        }).then(response => response.json())
            .then(data => {
                console.log(data);
            })
            .catch(error => {
                console.error('Error:', error);
            });
    } else {
        console.error("Failure");
    }
}

// Execute the main function
main();