"use strict";

$(document).ready(function () {
  bsCustomFileInput.init();

  const fileInputs = document.getElementsByName("files");
  for (let i = 0; i < fileInputs.length; i++) {
    fileInputs[i].addEventListener("change",
      (event) => {
        const value = fileInputs[i].parentNode.parentNode.children[0].value;
        if (value.substr(0, 10) !== "(Changed) ") {
          fileInputs[i].parentNode.parentNode.children[0].value = `(Changed) ${value}`;
        }
      });
  }

  document.getElementById("submitButton").addEventListener("click", validateFileInput);

  const fileInput = document.getElementById("newCustomFile");
  fileInput.addEventListener("change",
    (event) => {
      document.getElementById("fileValidationText").innerText = "";
    });

  function validateFileInput() {
    if (fileInput.value === "") {
      document.getElementById("fileValidationText").innerText = "Image is required.";
    }
  }

  const inputs = document.getElementsByTagName("input");
  for (let i = 0; i < inputs.length; i++) {
    inputs[i].addEventListener("input",
      (event) => {
        document.getElementById("notSavedWarning").hidden = false;
      });
  }
});