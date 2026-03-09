import { executeApiRequest, ApiRequestError } from './api';

describe('ApiRequestError', () => {
  it('stores the status code and message', () => {
    // Arrange & Act
    const error = new ApiRequestError('Not found', 404);

    // Assert
    expect(error.message).toBe('Not found');
    expect(error.statusCode).toBe(404);
    expect(error.name).toBe('ApiRequestError');
  });

  it('stores an optional response body', () => {
    // Arrange & Act
    const error = new ApiRequestError('Bad request', 400, '{"code":"ORG-VAL-001"}');

    // Assert
    expect(error.responseBody).toBe('{"code":"ORG-VAL-001"}');
  });

  it('is an instance of Error', () => {
    // Arrange & Act
    const error = new ApiRequestError('Server error', 500);

    // Assert
    expect(error).toBeInstanceOf(Error);
  });
});

describe('executeApiRequest', () => {
  beforeEach(() => {
    vi.resetAllMocks();
  });

  it('returns parsed JSON on a successful response', async () => {
    // Arrange
    const payload = { item: { status: 'healthy' } };
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: vi.fn().mockResolvedValue(payload),
    } as unknown as Response);

    // Act
    const result = await executeApiRequest<typeof payload>('/v1/health');

    // Assert
    expect(result).toEqual(payload);
  });

  it('sends Content-Type and Accept headers by default', async () => {
    // Arrange
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: vi.fn().mockResolvedValue({}),
    } as unknown as Response);

    // Act
    await executeApiRequest('/v1/health');

    // Assert
    const [, options] = (global.fetch as ReturnType<typeof vi.fn>).mock.calls[0] as [
      string,
      RequestInit,
    ];
    expect((options.headers as Record<string, string>)['Content-Type']).toBe('application/json');
    expect((options.headers as Record<string, string>)['Accept']).toBe('application/json');
  });

  it('throws ApiRequestError when the response is not ok', async () => {
    // Arrange
    global.fetch = vi.fn().mockResolvedValue({
      ok: false,
      status: 503,
      statusText: 'Service Unavailable',
      text: vi.fn().mockResolvedValue('{"code":"ORG-INT-001"}'),
    } as unknown as Response);

    // Act & Assert
    await expect(executeApiRequest('/v1/health')).rejects.toBeInstanceOf(ApiRequestError);
  });

  it('includes the HTTP status code in the thrown error', async () => {
    // Arrange
    global.fetch = vi.fn().mockResolvedValue({
      ok: false,
      status: 404,
      statusText: 'Not Found',
      text: vi.fn().mockResolvedValue(''),
    } as unknown as Response);

    // Act
    let caughtError: unknown;
    try {
      await executeApiRequest('/v1/missing');
    } catch (err) {
      caughtError = err;
    }

    // Assert
    expect(caughtError).toBeInstanceOf(ApiRequestError);
    expect((caughtError as ApiRequestError).statusCode).toBe(404);
  });

  it('prepends the VITE_API_BASE_URL env variable to the path', async () => {
    // Arrange — the default fallback is an empty string (no base URL)
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: vi.fn().mockResolvedValue({}),
    } as unknown as Response);

    // Act
    await executeApiRequest('/v1/health');

    // Assert
    const [url] = (global.fetch as ReturnType<typeof vi.fn>).mock.calls[0] as [string];
    expect(url).toContain('/v1/health');
  });
});
