export default interface AuthService {
  checkAuth: () => Promise<boolean>;
}
