// Toggle Profile Edit
function toggleProfileEdit() {
    const btn = document.getElementById('editProfileBtn');
    const inputs = document.querySelectorAll('.profile-input');

    if (btn.value === 'Edit Profile') {
        // Enable editing
        inputs.forEach(input => {
            input.removeAttribute('readonly');
            input.classList.add('border-primary');
        });
        btn.value = 'Save Changes';
        btn.classList.remove('btn-primary');
        btn.classList.add('btn-custom-green');
    } else {
        // Save changes
        inputs.forEach(input => {
            input.setAttribute('readonly', true);
            input.classList.remove('border-primary');
        });
        btn.value = 'Edit Profile';
        btn.classList.remove('btn-custom-green');
        btn.classList.add('btn-primary');
    }
}

// Toggle Contacts Edit
function toggleContactsEdit() {
    const btn = document.getElementById('editContactsBtn');
    const displays = document.querySelectorAll('.contact-display');
    const inputs = document.querySelectorAll('.contact-input');

    if (btn.textContent === 'Edit Contacts') {
        // Show inputs, hide displays
        displays.forEach(display => {
            display.classList.add('d-none');
        });
        inputs.forEach(input => {
            input.classList.remove('d-none');
        });
        btn.textContent = 'Save Changes';
        btn.classList.remove('btn-outline-primary');
        btn.classList.add('btn-custom-green');
    } else {
        // Save changes - update displays with input values
        const githubInput = document.getElementById('githubInput');
        const githubDisplay = document.getElementById('githubDisplay');

        if (githubInput && githubDisplay) {
            githubDisplay.textContent = githubInput.value;
        }

        // Hide inputs, show displays
        displays.forEach(display => {
            display.classList.remove('d-none');
        });
        inputs.forEach(input => {
            input.classList.add('d-none');
        });
        btn.textContent = 'Edit Contacts';
        btn.classList.remove('btn-custom-green');
        btn.classList.add('btn-outline-primary');
    }
}
