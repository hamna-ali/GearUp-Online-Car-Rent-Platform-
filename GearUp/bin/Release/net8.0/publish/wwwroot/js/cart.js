document.addEventListener('DOMContentLoaded', () => {
    const debounce = (fn, delay) => {
        let timer;
        return function (...args) {
            clearTimeout(timer);
            timer = setTimeout(() => fn.apply(this, args), delay);
        };
    };

    const updateCartItem = debounce((vehicleId) => {
        const noOfDays = document.querySelector(`input[name="noOfDays"][data-vehicle-id="${vehicleId}"]`)?.value || 1;
        const includeCarWash = document.querySelector(`input[name="includeCarWash"][data-vehicle-id="${vehicleId}"]`)?.checked || false;
        const includeCarDecor = document.querySelector(`input[name="includeCarDecor"][data-vehicle-id="${vehicleId}"]`)?.checked || false;

        fetch('/Cart/UpdateCartItem', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            },
            body: `vehicleId=${vehicleId}&noOfDays=${noOfDays}&includeCarWash=${includeCarWash}&includeCarDecor=${includeCarDecor}`
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    const totalPriceEl = document.getElementById('totalPrice');
                    if (totalPriceEl) {
                        totalPriceEl.textContent = data.updatedTotal;
                    }

                    // Update the item's total price in the cart
                    const itemTotalEl = document.querySelector(`.item-total[data-vehicle-id="${data.vehicleId}"]`);
                    if (itemTotalEl) {
                        itemTotalEl.textContent = data.itemTotal;
                    }
                } else {
                    alert("Failed to update cart item: " + data.message);
                }
            })

            .catch(err => {
                console.error("Error updating cart item:", err);
            });
    }, 600);

    document.querySelectorAll('.auto-update').forEach(input => {
        input.addEventListener('change', () => {
            const vehicleId = input.getAttribute('data-vehicle-id');
            updateCartItem(vehicleId);
        });
    });
});
