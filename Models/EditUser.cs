using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class BranchByUserId
    {
        public int branchId { get; set; }
        public int cityId { get; set; }
        public string branchName { get; set; }
    }
    public class BranchByUserIdRequest
    {
        public int userId { get; set; }
    }
    public class BranchByUserIdResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<BranchByUserId> branches { get; set; }
    }
    public class Reference
    {
        public string refName { get; set; }
        public string refCode { get; set; }
    }

    public class ReferenceResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<Reference> references { get; set; }
    }
    public class ReferenceRequest
    {
        public string refType { get; set; }
    }
    public class User
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int deptId { get; set; }
        public int desigId { get; set; }
        public string telNo1 { get; set; }
        public string telNo2 { get; set; }
        public string telNo3 { get; set; }
        public string emailAddress { get; set; }
        public bool isActive { get; set; }
        public bool isCharged { get; set; }
        public int auditId { get; set; }
        public int instId { get; set; }
        public int branchId { get; set; }
        public bool isPwdChanged { get; set; }
        public bool isBranchUser { get; set; }
        public int locality { get; set; }
        public string title { get; set; }
        public string certSerialNumber { get; set; }
        public string certificate { get; set; }
        public string certificateStatus { get; set; }
        public string country { get; set; }
        public string creationDateTime { get; set; }
        public bool isAuth { get; set; }
        public bool isRemCap { get; set; }
        public string active { get; set; }
        public string charged { get; set; }
        public int bAuth { get; set; }
    }

    public class UserRequest
    {
        public int userId { get; set; }
    }
    public class UserResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<User> users { get; set; }
    }
    public class UserRole
    {
        public int userId { get; set; }
        public int roleId { get; set; }
        public string creationDateTime { get; set; }
    }

    public class UserRoleResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<UserRole> roles { get; set; }
    }
    public class EmailRequest
    {
        public string email { get; set; }
    }
    public class isUserExistRequest
    {
        public int instId { get; set; }
        public int branchId { get; set; }
        public string email { get; set; }
    }
    public class UserNameRequest2
    {
        public string userName { get; set; }
    }
    public class InsertUserRequest
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int instId { get; set; }
        public int branchId { get; set; }
        public int deptId { get; set; }
        public int desigId { get; set; }
        public string telNo1 { get; set; }
        public string telNo2 { get; set; }
        public string telNo3 { get; set; }
        public string emailAddress { get; set; }
        public bool isActive { get; set; }
        public bool isCharged { get; set; }
        public bool isPwdChanged { get; set; }
        public bool isBranchUser { get; set; }
        public int locality { get; set; }
        public string title { get; set; }
        public string country { get; set; }
        public bool isAuth { get; set; }
        public string certSerialNumber { get; set; }
        public string certificate { get; set; }
        public string certificateStatus { get; set; }
    }
    public class InsertBankCertificateRequest
    {
        public string subjectEmail { get; set; }
        public string startDate { get; set; }
        public int bankCode { get; set; }
        public int branchCode { get; set; }
        public string subjectDN { get; set; }
        public string expiryDate { get; set; }
        public string base64Cert { get; set; }
    }
    public class UpdateUserRequest : InsertUserRequest
    {
        public int userId { get; set; }
    }
    public class UserDetailPostviaLogin
    {
        public string user { get; set; }
        public string pass { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string EmailAddress { get; set; }
        public string SerialNo { get; set; }
    }
    public class UserDetailviaLogin
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string Password { get; set; }
        public string fullName { get; set; }
        public string instID { get; set; }
        public string branchID { get; set; }
        public bool Isbranchuser { get; set; }
        public string emailAddress { get; set; }
        public string SerialNo { get; set; }
        public string instName { get; set; }
        public string timeIn { get; set; }
        public string UserRole { get; set; }
        public string RoleName { get; set; }
        public bool IspwdChanged { get; set; }
        public DateTime LockTime { get; set; }
        public bool IsLock { get; set; }
        public string userlogid { get; set; }
    }
    public class UserDetailResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<UserDetailviaLogin> UdList { get; set; }
    }
    public class IsLoggedResult
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public int isLogged { get; set;}
    }
    public class FirstTime_UserDetail
    {
        public string userId { get; set; }
        public string cnic {  get; set; }
        public string dob { get; set; }
        public string ques { get; set; }
        public string ans { get; set; }
        public string pass1 { get; set; }
        public string pass2 { get; set; }
    }
    public class FirstTime_UserResponse
    {
        public bool FT_Login { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }
}