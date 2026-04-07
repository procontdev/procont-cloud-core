export interface TenantDto {
  id: string;
  code: string;
  name: string;
  isActive: boolean;
}

export interface CompanyDto {
  id: string;
  tenantId: string;
  legalName: string;
  taxId: string;
}

export interface UserDto {
  id: string;
  tenantId: string;
  email: string;
  displayName: string;
  role: "owner" | "admin" | "accountant" | "auditor";
}
