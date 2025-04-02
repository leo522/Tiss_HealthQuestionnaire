document.addEventListener("DOMContentLoaded", function () {
    function validateFillDate() {
        const fillDateInput = document.querySelector("input[name='fillDate']");

        if (!fillDateInput.value.trim()) {
            Swal.fire({
                icon: "error",
                title: "填表日期未填寫",
                text: "請選擇填表日期後再提交！",
                confirmButtonText: "確定"
            });
            
            fillDateInput.scrollIntoView({ behavior: "smooth", block: "center" });

            return false;
        }

        return true;
    }

    document.getElementById("submitButton").addEventListener("click", function (event) {
        if (!validateFillDate()) {
            event.preventDefault();
        }
    });
});