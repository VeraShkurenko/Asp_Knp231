document.addEventListener('DOMContentLoaded', () => {
    const adminDiscountBtn = document.getElementById("admin-discount-btn");
    if (adminDiscountBtn) {
        adminDiscountBtn.addEventListener('click', adminDiscountClick);
    }

    const adminDiscountDetailBtn = document.getElementById("discount-detail-btn");
    if (adminDiscountDetailBtn) {
        adminDiscountDetailBtn.addEventListener('click', adminDiscountDetailClick);
    }
});

function clearValidation(form) {
    form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
    form.querySelectorAll('[data-valmsg-for]').forEach(el => el.textContent = '');
}

function showValidation(form, errorsMap) {
    for (const key in errorsMap) {
        const errorBlock = form.querySelector(`[data-valmsg-for="${key}"]`);
        if (errorBlock) {
            errorBlock.textContent = errorsMap[key].join(', ');
        }

        let field = null;

        switch (key) {
            case 'Title':
                field = form.querySelector('[name="discount-title"]');
                break;
            case 'Description':
                field = form.querySelector('[name="discount-description"]');
                break;
            case 'Percent':
                field = form.querySelector('[name="discount-percent"]');
                break;
            case 'Price':
                field = form.querySelector('[name="discount-price"], [name="discount-detail-price"]');
                break;
            case 'Start':
                field = form.querySelector('[name="discount-start"]');
                break;
            case 'Finish':
                field = form.querySelector('[name="discount-finish"]');
                break;
            case 'DiscountId':
                field = form.querySelector('[name="discount-detail-discount-id"]');
                break;
            case 'ProductId':
                field = form.querySelector('[name="discount-detail-product-id"]');
                break;
        }

        if (field) {
            field.classList.add('is-invalid');
        }
    }
}

function parseModelState(modelState) {
    const result = {};

    for (const key in modelState) {
        if (modelState[key].errors && modelState[key].errors.length > 0) {
            result[key] = modelState[key].errors.map(e => e.errorMessage);
        }
        else if (modelState[key].Errors && modelState[key].Errors.length > 0) {
            result[key] = modelState[key].Errors.map(e => e.ErrorMessage);
        }
    }

    return result;
}

function adminDiscountDetailClick(e) {
    const form = e.target.closest("form");
    if (!form) throw "adminDiscountDetailClick: Closest form not found";

    clearValidation(form);

    const formData = new FormData(form);

    fetch("/Shop/DiscountDetailFormReceiver", {
        method: "POST",
        body: formData
    })
        .then(r => r.json())
        .then(j => {
            if (typeof j.status !== 'undefined' && j.status === 'OK') {
                window.location.reload();
            }
            else {
                showValidation(form, parseModelState(j));
            }
        });
}

function adminDiscountClick(e) {
    const form = e.target.closest("form");
    if (!form) throw "adminDiscountClick: Closest form not found";

    clearValidation(form);

    const formData = new FormData(form);

    fetch("/Shop/DiscountFormReceiver", {
        method: "POST",
        body: formData
    })
        .then(r => r.json())
        .then(j => {
            if (typeof j.status !== 'undefined' && j.status === 'OK') {
                window.location.reload();
            }
            else {
                showValidation(form, parseModelState(j));
            }
        });
}