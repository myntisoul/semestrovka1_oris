const searchForm = document.querySelector("form.search");
const searchInput = searchForm.elements.namedItem("q");
const wikiContent = document.getElementById("wiki-content");
const searchStatus = document.getElementById("search-status");
let pendingSearch;

if (location.pathname === new URL(searchForm.action).pathname) {
    searchInput.value = new URLSearchParams(location.search).get("q") ?? "";
}

searchForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    pendingSearch?.abort();
    const request = new AbortController();
    pendingSearch = request;

    const pageUrl = new URL(searchForm.action);
    pageUrl.searchParams.set("q", searchInput.value);
    const fragmentUrl = new URL(pageUrl);
    fragmentUrl.searchParams.set("fragment", "1");

    searchStatus.hidden = false;
    searchStatus.textContent = "Поиск…";
    wikiContent.setAttribute("aria-busy", "true");

    try {
        const response = await fetch(fragmentUrl, { signal: request.signal });
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        const html = await response.text();
        if (request.signal.aborted) return;

        if (pageUrl.href !== location.href) {
            history.pushState(null, "", pageUrl);
        }
        wikiContent.innerHTML = html;
        searchStatus.textContent = "Результаты поиска обновлены.";
    } catch (error) {
        if (!request.signal.aborted) {
            searchStatus.textContent = "Не удалось выполнить поиск. Попробуйте ещё раз.";
        }
    } finally {
        if (pendingSearch === request) {
            pendingSearch = undefined;
            wikiContent.removeAttribute("aria-busy");
        }
    }
});

window.addEventListener("popstate", () => location.reload());
