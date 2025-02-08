/*過去傷勢*/
document.addEventListener("DOMContentLoaded", function () {
    const pastNoInjury = document.getElementById("pastNoInjury");
    const pastHasInjury = document.getElementById("pastHasInjury");
    const injuryPartDetails = document.getElementById("injuryPartDetails");
    const pastInjuryTypeContainer = document.querySelector("#allInjuryTypesSection .grid");
    const treatmentMethodContainer = document.querySelector("#treatmentMethodSection .grid");

    const injuryTypesInput = document.getElementById("selectedInjuryTypes");
    const treatmentMethodsInput = document.getElementById("selectedTreatmentMethods");

    function updateHiddenInputs() {
        injuryTypesInput.value = Array.from(document.querySelectorAll(".past-injury-type"))
            .map(select => select.value)
            .filter(value => value)
            .join(",");

        treatmentMethodsInput.value = Array.from(document.querySelectorAll('input[name="treatmentMethod"]:checked'))
            .map(checkbox => checkbox.value)
            .join(",");
    }

    document.getElementById("questionnaireForm").addEventListener("submit", updateHiddenInputs);
});
