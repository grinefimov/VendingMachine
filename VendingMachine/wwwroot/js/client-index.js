$(document).ready(function() {
  const products = [];
  const cashes = [];
  const checkout = [];
  const total = {
    value: 0,
    get get() { return this.value; },
    set set(value) {
      this.value = value;
      document.getElementById("total").innerText = value + " rub";
      change.updateValue();
    }
  };
  const balance = {
    value: 0,
    get get() { return this.value; },
    set set(value) {
      this.value = value;
      document.getElementById("balance").innerText = value + " rub";
      change.updateValue();
    }
  };
  const change = {
    value: 0,
    get get() { return this.value; },
    set set(value) {
      this.value = value;
      document.getElementById("change").innerText = value + " rub";
    },
    updateValue: function() {
      this.set = balance.get - total.get;
    }
  };
  const newCashIds = [];
  const cards = document.getElementsByName("productCard");
  const cashButtons = document.getElementsByName("cashButton");
  const buyButton = document.getElementById("buyButton");
  const checkoutDiv = document.getElementById("checkout");

  $.ajax({
    url: '/Client/GetData',
    type: 'GET',
    success: function(response) {
      for (let i = 0; i < response.products.length; i++) {
        products.push({
          id: response.products[i].id,
          name: response.products[i].name,
          price: response.products[i].price,
          quantity: response.products[i].quantity,
          imageUrl: response.products[i].imageUrl
        });
      }
      for (let i = 0; i < response.cashes.length; i++) {
        cashes.push({
          id: response.cashes[i].id,
          faceValue: response.cashes[i].faceValue,
          quantity: response.cashes[i].quantity
        });
      }
    },
    error: function() {
      throw "AJAX error";
    }
  });

  for (let i = 0; i < cards.length; i++) {
    cards[i].addEventListener("click",
      (event) => {
        if (products[i].quantity > 0) {
          if (total.get + products[i].price <= balance.get) {
            products[i].quantity -= 1;
            total.set = total.get + products[i].price;
            checkout.push(products[i]);

            const div = document.createElement("div");
            div.innerHTML = `
              <div class="row">
                <div class="col">${products[i].name}</div>
                <div class="col font-italic text-right">${products[i].price} rub</div>
              </div>`;
            checkoutDiv.appendChild(div);
          } else {
            modalAlert("Deposit money!");
          }
        } else {
          modalAlert("Sold out!");
        }
      });
  }
  for (let i = 0; i < cashButtons.length; i++) {
    cashButtons[i].addEventListener("click",
      (event) => {
        newCashIds.push(cashes[i].id);
        balance.set = balance.get + cashes[i].faceValue;
      });
  }

  function modalAlert(message) {
    document.getElementById("modalMessage").innerText = message;
    document.getElementById("messageModalButton").click();
  }

  buyButton.addEventListener("click",
    (event) => {
      if (checkout.length || balance.get > 0) {
        $.ajax({
          url: "/Client/Buy",
          data: { checkout: checkout, newCashIds: newCashIds, change: change.get },
          type: "POST",
          success: function(data) {
            newCashIds.length = 0;
            checkout.length = 0;
            checkoutDiv.innerText = "";
            total.set = 0;
            balance.set = 0;
            change.set = 0;
            let noCashLeftMessage = "";
            if (data.noCashLeft) {
              noCashLeftMessage = " No Cash Left";
            }
            modalAlert(`Your change: ${data.change} rub (Cashes: ${data.changeCashes}${noCashLeftMessage})`);
          },
          error: function() {
            throw "AJAX error";
          }
        });
      }
    });
});