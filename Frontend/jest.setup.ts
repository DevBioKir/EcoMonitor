// Мок для fetch
global.fetch = jest.fn();

// Мок для FormData._parts
(global as any).FormData = class FormData {
  _parts: any[] = [];
  append(key: string, value: any) {
    this._parts.push([key, value]);
  }
} as any;