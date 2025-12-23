These conventions keep Fluent UI and Syncfusion components in sync with EF models inside the .NET 10 Blazor host. Follow them to avoid double updates, event floods, or validation regressions.

## Universal rules
1. **Bind + callback** - pair `@bind-*` with a method (`@bind-Value:after`, `OnChange`, etc.) when the UI writes to the database.
2. **Async first** - callbacks that touch services or EF must return `Task` and be awaited. Avoid `async void`.
3. **PascalCase attributes** - Fluent/Syncfusion components use PascalCase (`OnChange`). DOM events stay lowercase (`@oninput`).
4. **Small lambdas** - keep inline lambdas to one statement (e.g., `@(args => SaveChanges())`). Move logic into a method when it grows.
5. **Immediate for filters** - prefer built-in immediate bindings over manual `@oninput` where possible.

## Common patterns
| Control | Binding | Change hook | Example |
| --- | --- | --- | --- |
| `FluentTextField` | `@bind-Value` | `@bind-Value:after="ApplyFiltersAndRefreshAsync"` | `Domain/Attachments/Components/AttachmentListGrid.razor` |
| `FluentCheckbox` | `@bind-Value` | `OnChange="OnCheckedChanged"` | `Domain/Profile/Pages/Templates.razor` |
| `FluentRadioGroup` | `@bind-Value` | `ValueChanged="OnOptionChanged"` | `Domain/Profile/Pages/Profile.razor` |
| `SfDropDownList` | `@bind-Value` | `@bind-Value:after="OnSelectionChange"` + `<DropDownListEvents ValueChange="OnSelectionChange" />` | `Domain/Policies/Pages/Edit.razor` |
| `SfNumericTextBox` | `@bind-Value` | `OnChange="@(args => SaveChanges())"` | `Domain/Clients/Components/BusinessDetails.razor` |
| `SfDatePicker` | `@bind-Value` | `OnChange="@(args => SaveChanges())"` | `Domain/Clients/Components/BusinessDetails.razor` |

## Fluent UI specifics
- **TextField/TextArea** - use `Immediate="true"` for filters; for expensive operations use `@bind-Value:after` to debounce inside your method.
- **NumberField** - `ValueChanged` + `ValueExpression` are required when used inside `EditForm`.
- **Checkbox** - `@bind-Value` works for boolean states; avoid mixing with `@bind-Checked`.
- **Combobox** - prefer Fluent Combobox for short lists, Syncfusion for large datasets.

## Syncfusion specifics
### Imports
```razor
@using Syncfusion.Blazor
@using Syncfusion.Blazor.DropDowns
@using Syncfusion.Blazor.Inputs
```

### DropDowns
```razor
<SfDropDownList TItem="Carrier" TValue="int?"
                DataSource="@Carriers"
                @bind-Value="SelectedCarrierId"
                AllowFiltering="true"
                @bind-Value:after="OnCarrierChanged">
    <DropDownListFieldSettings Text="CarrierName" Value="CarrierId" />
    <DropDownListEvents TValue="int?" TItem="Carrier"
                        ValueChange="OnCarrierChanged" />
</SfDropDownList>
```
- Generics (`TItem`, `TValue`) must match `DataSource` and bound property.
- Use `<DropDownListEvents>` for `ValueChange`; do not put `ValueChange` on the root element.

### DatePicker
```razor
<SfDatePicker TValue="DateTime?"
              @bind-Value="Renewal.RenewalDate"
              OnChange="@(args => SaveChanges())" />
```
- Use a lambda when ignoring event args; direct method references fail because Syncfusion expects a parameter.

### NumericTextBox
```razor
<SfNumericTextBox TValue="decimal?"
                  @bind-Value="Policy.Premium"
                  Format="C2"
                  OnChange="@(args => SaveChanges())" />
```
- Avoid `ValueChange` (wrong event) - use `OnChange` or `@bind-Value:after`.

## Validation helpers
- Use `<EditForm>` + `<DataAnnotationsValidator>` where possible.
- For Syncfusion components inside forms, set `ValueExpression="() => Model.Property"`.

## Anti-patterns to avoid
- Mixing DOM events (`@onchange`) with component events on the same control
- Self-closing `<SfCheckBox />` tags (Syncfusion needs explicit closing tags)
- `async void` callbacks
- Passing `null` `DataSource` to `SfDropDownList` - pre-initialize with `Array.Empty<T>()`

## References
- `Domain/Attachments/Components/AttachmentListGrid.razor` shows Fluent filters + `@bind-Value:after`.
- `Domain/Policies/Pages/Edit.razor` demonstrates dropdown + date + numeric patterns.
- `Domain/Clients/Components/BusinessDetails.razor` shows Syncfusion inputs with `OnChange`/`@onblur`.

Use this doc as the authoritative source before copy/pasting snippets so generated code compiles the first time.
