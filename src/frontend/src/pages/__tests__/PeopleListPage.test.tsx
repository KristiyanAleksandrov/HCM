import { render, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import PeopleListPage from "../PeopleListPage";
import api from "../../apis/peopleApi";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { BrowserRouter } from "react-router-dom";
import { AuthContext } from "../../contexts/AuthContext";

vi.mock("../../apis/peopleApi");

const people = [
  {
    id: "1",
    firstName: "kris",
    lastName: "aleksandrov",
    email: "kris@gmail.com",
    position: "Engineer",
  },
];

function renderPage(role = "HRAdmin") {
  return render(
    <AuthContext.Provider
      value={{
        token: "T",
        role,
        user: "Tester",
        logout: vi.fn(),
        login: vi.fn(),
      }}
    >
      <BrowserRouter>
        <PeopleListPage />
      </BrowserRouter>
    </AuthContext.Provider>
  );
}

describe("PeopleListPage", () => {
  beforeEach(() => {
    (api.get as any).mockResolvedValueOnce({ data: people });
  });

  it("renders a table of people", async () => {
    renderPage();
    expect(screen.getByText(/people records/i)).toBeInTheDocument();
    await waitFor(() => expect(api.get).toHaveBeenCalledWith("/people"));
    const row = await screen.findByRole("row", { name: /kris/i });

    within(row).getByRole("link", { name: /edit/i });
    within(row).getByRole("button", { name: /delete/i });
  });

  it("hides action buttons for basic users", async () => {
    renderPage("Employee");

    const row = await screen.findByRole("row", { name: /kris/i });

    expect(
      within(row).queryByRole("link", { name: /edit/i })
    ).not.toBeInTheDocument();
    expect(
      within(row).queryByRole("button", { name: /delete/i })
    ).not.toBeInTheDocument();
  });

  it("deletes a person after confirmation", async () => {
    vi.spyOn(window, "confirm").mockReturnValue(true);
    (api.get as any).mockResolvedValueOnce({ data: people });
    (api.delete as any).mockResolvedValueOnce({});

    renderPage();

    const row = await screen.findByRole("row", { name: /kris/i });

    const deleteButton = within(row).getByRole("button", { name: /delete/i });
    await userEvent.click(deleteButton);

    await waitFor(() =>
      expect(api.delete).toHaveBeenCalledWith("/people?id=1")
    );
  });
});
