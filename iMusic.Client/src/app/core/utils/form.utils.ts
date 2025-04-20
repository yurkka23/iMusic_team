export function convertJsonToFormData(formValue: Object, carryFormData?: FormData): FormData {
  const formData = carryFormData || new FormData();

  for (const key in formValue) {
    if (formValue[key] !== null && formValue[key] !== undefined) {
      if (isObject(formValue[key]) && !(formValue[key] instanceof File) && !(formValue[key] instanceof Date)) {
        convertJsonToFormData(formValue[key], formData);
      } else if (Array.isArray(formValue[key])) {
        formValue[key].forEach((value): void => {
          formData.append(key, value);
        });
      } else {
        formData.append(key, formValue[key]);
      }
    }
  }

  return formData;
}

function isObject(value): boolean {
  return !Array.isArray(value) && typeof value === 'object' && !!value;
}
