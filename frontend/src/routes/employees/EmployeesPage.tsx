import { useEffect, useState } from 'react';
import { executeApiRequest } from '../../lib/api';
import type { CollectionEnvelope, EmployeeInfo } from '../../lib/types';
import { LoadingSpinner } from '../../components/LoadingSpinner';
import { ErrorMessage } from '../../components/ErrorMessage';

type PageState = 'loading' | 'success' | 'error';

/**
 * Employee directory page.
 * Fetches and displays the employee list with department filtering and pagination info.
 */
export function EmployeesPage() {
  const [pageState, setPageState] = useState<PageState>('loading');
  const [employeesData, setEmployeesData] = useState<CollectionEnvelope<EmployeeInfo> | null>(null);
  const [errorText, setErrorText] = useState<string>('');
  const [selectedDepartment, setSelectedDepartment] = useState<string>('');
  const [availableDepartments, setAvailableDepartments] = useState<string[]>([]);

  useEffect(() => {
    let isMounted = true;

    async function fetchEmployees() {
      setPageState('loading');
      try {
        const departmentParam = selectedDepartment
          ? `&department=${encodeURIComponent(selectedDepartment)}`
          : '';

        const apiResponse = await executeApiRequest<CollectionEnvelope<EmployeeInfo>>(
          `/v1/employees?limit=20&offset=0${departmentParam}`
        );

        if (isMounted) {
          setEmployeesData(apiResponse);

          // Populate the department dropdown from the unfiltered initial fetch
          if (!selectedDepartment) {
            const uniqueDepts = [...new Set(apiResponse.items.map((e) => e.department))].sort();
            setAvailableDepartments(uniqueDepts);
          }

          setPageState('success');
        }
      } catch (err) {
        if (isMounted) {
          const errorMessage = err instanceof Error ? err.message : 'Unknown error occurred';
          setErrorText(errorMessage);
          setPageState('error');
        }
      }
    }

    fetchEmployees();

    return () => {
      isMounted = false;
    };
  }, [selectedDepartment]);

  if (pageState === 'loading') {
    return <LoadingSpinner />;
  }

  if (pageState === 'error') {
    return (
      <ErrorMessage
        errorTitle="Failed to Load Employees"
        errorDescription={errorText}
      />
    );
  }

  if (!employeesData) {
    return null;
  }

  const { items: employees, metadata } = employeesData;

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-bold text-[var(--color-brand-text)]">
        Employee Directory
      </h2>

      {/* Filters bar */}
      <div className="flex items-center gap-4 bg-white rounded-lg shadow-sm p-4">
        <label
          htmlFor="department-filter"
          className="text-sm font-medium text-gray-600 whitespace-nowrap"
        >
          Filter by Department:
        </label>
        <select
          id="department-filter"
          value={selectedDepartment}
          onChange={(e) => setSelectedDepartment(e.target.value)}
          className="border border-gray-300 rounded-md px-3 py-1.5 text-sm text-gray-700 bg-white focus:outline-none focus:ring-2 focus:ring-[var(--color-brand-primary)] focus:border-transparent"
        >
          <option value="">All Departments</option>
          {availableDepartments.map((dept) => (
            <option key={dept} value={dept}>
              {dept}
            </option>
          ))}
        </select>

        {selectedDepartment && (
          <button
            onClick={() => setSelectedDepartment('')}
            className="text-sm text-[var(--color-brand-primary)] hover:underline"
          >
            Clear filter
          </button>
        )}

        <span className="ml-auto text-sm text-gray-500">
          {metadata.totalCount !== undefined
            ? `${metadata.totalCount} employee${metadata.totalCount !== 1 ? 's' : ''} found`
            : `${employees.length} employee${employees.length !== 1 ? 's' : ''} shown`}
        </span>
      </div>

      {/* Employee table */}
      {employees.length === 0 ? (
        <div className="bg-white rounded-lg shadow-md p-8 text-center text-gray-500">
          No employees match the selected filter.
        </div>
      ) : (
        <div className="bg-white rounded-lg shadow-md overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="text-left px-6 py-3 font-semibold text-gray-600">Full Name</th>
                <th className="text-left px-6 py-3 font-semibold text-gray-600">Email</th>
                <th className="text-left px-6 py-3 font-semibold text-gray-600">Department</th>
                <th className="text-left px-6 py-3 font-semibold text-gray-600">Job Title</th>
                <th className="text-left px-6 py-3 font-semibold text-gray-600">Hire Date</th>
                <th className="text-left px-6 py-3 font-semibold text-gray-600">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {employees.map((employee) => (
                <tr
                  key={employee.employeeId}
                  className="hover:bg-gray-50 transition-colors"
                >
                  <td className="px-6 py-4 font-medium text-[var(--color-brand-text)]">
                    {employee.fullName}
                  </td>
                  <td className="px-6 py-4 text-gray-600">
                    <a
                      href={`mailto:${employee.email}`}
                      className="hover:text-[var(--color-brand-primary)] hover:underline"
                    >
                      {employee.email}
                    </a>
                  </td>
                  <td className="px-6 py-4 text-gray-600">{employee.department}</td>
                  <td className="px-6 py-4 text-gray-600">{employee.jobTitle}</td>
                  <td className="px-6 py-4 text-gray-500">
                    {employee.hireDate
                      ? new Date(employee.hireDate).toLocaleDateString()
                      : '—'}
                  </td>
                  <td className="px-6 py-4">
                    {employee.activeIndicator ? (
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 border border-green-300">
                        Active
                      </span>
                    ) : (
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-600 border border-gray-300">
                        Inactive
                      </span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Response metadata footer */}
      <div className="bg-gray-50 rounded-lg p-4">
        <h3 className="text-sm font-semibold text-gray-600 mb-2">Response Metadata</h3>
        <p className="text-sm text-gray-500">
          <span className="font-medium">Transaction ID:</span> {metadata.transactionId}
        </p>
        <p className="text-sm text-gray-500">
          <span className="font-medium">Timestamp:</span> {metadata.timestamp}
        </p>
      </div>
    </div>
  );
}
