using UTM.Gamepad.BussinessLogic.Services;
using UTM.Gamepad.BussinessLogic.Services.Interfaces;

namespace UTM.Gamepad.BussinessLogic
{
    public class BusinessLogicFactory
    {
        private static BusinessLogicFactory _instance;
        private static readonly object _lock = new object();

        private IAuthBL _authBL;
        private IOrderBL _orderBL;
        private IUserBL _userBL;
        private IRoleBL _roleBL;
        private IProductBL _productBL;

        private BusinessLogicFactory() { }

        public static BusinessLogicFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BusinessLogicFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        public IAuthBL GetAuthBL()
        {
            if (_authBL == null)
            {
                _authBL = new AuthBL();
            }
            return _authBL;
        }

        public IOrderBL GetOrderBL()
        {
            if (_orderBL == null)
            {
                _orderBL = new OrderBL();
            }
            return _orderBL;
        }
        
        public IUserBL GetUserBL()
        {
            if (_userBL == null)
            {
                _userBL = new UserBL();
            }
            return _userBL;
        }
        
        public IRoleBL GetRoleBL()
        {
            if (_roleBL == null)
            {
                _roleBL = new RoleBL();
            }
            return _roleBL;
        }
        
        public IProductBL GetProductBL()
        {
            if (_productBL == null)
            {
                _productBL = new ProductBL();
            }
            return _productBL;
        }
    }
} 