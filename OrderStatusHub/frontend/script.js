//const API_BASE = (window.location.hostname === "localhost") ? "http://localhost:5000/api" : "/api";
const API_BASE = "https://localhost:63174/api/order-hub";

async function fetchOrders(statusFilter = "") {
  let url = `${API_BASE}/orders`;
  if (statusFilter) {
    url = `${API_BASE}/orders/search?status=${encodeURIComponent(statusFilter)}`;
  }

  try {
    const res = await fetch(url);

    // Log status for debugging
    console.log(`API Request: ${url} | Status: ${res.status}`);

    // If the backend returns a non-200 status, show proper error
    if (!res.ok) {
      const errorMsg = await extractError(res);
      throw new Error(`API returned ${res.status}: ${errorMsg}`);
    }

    return await res.json();
  } catch (err) {
    // Console logging for debugging
    console.error("Fetch error:", err);
    throw err;
  }
}

async function extractError(res) {
  try {
    const json = await res.json();
    return json.message || JSON.stringify(json);
  } catch {
    return "Unknown error";
  }
}

function renderOrders(orders) {
  const tbody = document.querySelector("#ordersTable tbody");
  tbody.innerHTML = "";
  for (const o of orders) {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td>${escapeHtml(o.orderId)}</td>
      <td>${escapeHtml(o.sourceSystem)}</td>
      <td>${escapeHtml(o.customerName)}</td>
      <td>${escapeHtml(o.orderDate)}</td>
      <td>${Number(o.totalAmount).toFixed(2)}</td>
      <td>${escapeHtml(o.status)}</td>
    `;
    tbody.appendChild(tr);
  }
}

function escapeHtml(text) {
  if (text == null) return "";
  return text.toString()
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
}

document.getElementById("applyFilter").addEventListener("click", async () => {
  const status = document.getElementById("statusFilter").value;
  await loadOrders(status);
});

document.getElementById("reload").addEventListener("click", async () => {
  document.getElementById("statusFilter").value = "";
  await loadOrders();
});

async function loadOrders(status = "") {
  const errEl = document.getElementById("error");
  errEl.hidden = true;

  try {
    const orders = await fetchOrders(status);
    renderOrders(orders);
  } catch (err) {
    errEl.hidden = false;
    errEl.textContent = "Failed to load orders: " + err.message;

    // Detailed console error
    console.error("Error while loading orders:", err);
  }
}

window.addEventListener("load", async () => {
  console.log("Frontend initialized. Loading orders...");
  await loadOrders();
});

