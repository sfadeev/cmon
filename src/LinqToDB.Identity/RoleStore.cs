// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Data;
using Microsoft.AspNetCore.Identity;

namespace LinqToDB.Identity
{
	/// <summary>
	///     Creates a new instance of a persistence store for roles.
	/// </summary>
	/// <typeparam name="TRole">The type of the class representing a role.</typeparam>
	/// <typeparam name="TContext">
	///     The type of the class for <see cref="IDataContext" />,
	///     <see cref="IConnectionFactory{TContext,TConnection}" />
	/// </typeparam>
	/// <typeparam name="TConnection">
	///     The type of the class for <see cref="DataConnection" />,
	///     <see cref="IConnectionFactory{TContext,TConnection}" />
	/// </typeparam>
	public class RoleStore<TContext, TConnection, TRole> : RoleStore<TContext, TConnection, TRole, string>
		where TRole : IdentityRole<string>
		where TContext : IDataContext
		where TConnection : DataConnection
	{
		/// <summary>
		///     Constructs a new instance of <see cref="RoleStore{TRole, TContext, TConnection}" />.
		/// </summary>
		/// <param name="factory">
		///     <see cref="IConnectionFactory{TContext,TConnection}" />
		/// </param>
		/// <param name="describer">The <see cref="IdentityErrorDescriber" />.</param>
		public RoleStore(IConnectionFactory<TContext, TConnection> factory, IdentityErrorDescriber describer = null)
			: base(factory, describer)
		{
		}
	}

	/// <summary>
	///     Creates a new instance of a persistence store for roles.
	/// </summary>
	/// <typeparam name="TRole">The type of the class representing a role.</typeparam>
	/// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
	/// <typeparam name="TContext">
	///     The type of the class for <see cref="IDataContext" />,
	///     <see cref="IConnectionFactory{TContext,TConnection}" />
	/// </typeparam>
	/// <typeparam name="TConnection">
	///     The type of the class for <see cref="DataConnection" />,
	///     <see cref="IConnectionFactory{TContext,TConnection}" />
	/// </typeparam>
	public class RoleStore<TContext, TConnection, TRole, TKey> :
			RoleStore<TContext, TConnection, TRole, TKey, IdentityUserRole<TKey>, IdentityRoleClaim<TKey>>,
			IQueryableRoleStore<TRole>,
			IRoleClaimStore<TRole>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TContext : IDataContext
		where TConnection : DataConnection
	{
		/// <summary>
		///     Constructs a new instance of <see cref="RoleStore{TRole, TKey, TContext, TConnection}" />.
		/// </summary>
		/// <param name="factory">
		///     <see cref="IConnectionFactory{TContext,TConnection}" />
		/// </param>
		/// <param name="describer">The <see cref="IdentityErrorDescriber" />.</param>
		public RoleStore(IConnectionFactory<TContext, TConnection> factory, IdentityErrorDescriber describer = null)
			: base(factory, describer)
		{
		}

		/// <summary>
		///     Creates a entity representing a role claim.
		/// </summary>
		/// <param name="role">The associated role.</param>
		/// <param name="claim">The associated claim.</param>
		/// <returns>The role claim entity.</returns>
		protected override IdentityRoleClaim<TKey> CreateRoleClaim(TRole role, Claim claim)
		{
			var roleClaim = new IdentityRoleClaim<TKey> {RoleId = role.Id};
			roleClaim.InitializeFromClaim(claim);
			return roleClaim;
		}
	}

	/// <summary>
	///     Creates a new instance of a persistence store for roles.
	/// </summary>
	/// <typeparam name="TRole">The type of the class representing a role.</typeparam>
	/// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
	/// <typeparam name="TUserRole">The type of the class representing a user role.</typeparam>
	/// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
	/// <typeparam name="TContext">
	///     The type of the class for <see cref="IDataContext" />,
	///     <see cref="IConnectionFactory{TContext,TConnection}" />
	/// </typeparam>
	/// <typeparam name="TConnection">
	///     The type of the class for <see cref="DataConnection" />,
	///     <see cref="IConnectionFactory{TContext,TConnection}" />
	/// </typeparam>
	public abstract class RoleStore<TContext, TConnection, TRole, TKey, TUserRole, TRoleClaim> :
			IQueryableRoleStore<TRole>,
			IRoleClaimStore<TRole>
		where TRole : class, IIdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TUserRole : class, IIdentityUserRole<TKey>
		where TRoleClaim : class, IIdentityRoleClaim<TKey>
		where TContext : IDataContext
		where TConnection : DataConnection
	{
		private readonly IConnectionFactory<TContext, TConnection> _factory;

		private bool _disposed;

		/// <summary>
		///     Constructs a new instance of <see cref="RoleStore{TRole, TKey, TUserRole, TRoleClaim, TContext, TConnection}" />.
		/// </summary>
		/// <param name="factory">
		///     <see cref="IConnectionFactory{TContext,TConnection}" />
		/// </param>
		/// <param name="describer">The <see cref="IdentityErrorDescriber" />.</param>
		public RoleStore(IConnectionFactory<TContext, TConnection> factory, IdentityErrorDescriber describer = null)
		{
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			_factory = factory;

			ErrorDescriber = describer ?? new IdentityErrorDescriber();
		}


		/// <summary>
		///     Gets the database context for this store.
		/// </summary>
		private IDataContext Context => _factory.GetContext();

		/// <summary>
		///     Gets or sets the <see cref="IdentityErrorDescriber" /> for any error that occurred with the current operation.
		/// </summary>
		public IdentityErrorDescriber ErrorDescriber { get; set; }


		/// <summary>
		///     Creates a new role in a store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to create in the store.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>A <see cref="Task{TResult}" /> that represents the <see cref="IdentityResult" /> of the asynchronous query.</returns>
		public virtual async Task<IdentityResult> CreateAsync(TRole role,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));


			await Task.Run(() => Context.TryInsertAndSetIdentity(role), cancellationToken);
			return IdentityResult.Success;
		}

		/// <summary>
		///     Updates a role in a store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to update in the store.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>A <see cref="Task{TResult}" /> that represents the <see cref="IdentityResult" /> of the asynchronous query.</returns>
		public virtual async Task<IdentityResult> UpdateAsync(TRole role,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));

			var result = await Task.Run(() => _factory.GetContext().UpdateConcurrent<TRole, TKey>(role), cancellationToken);
			return result == 1 ? IdentityResult.Success : IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
		}

		/// <summary>
		///     Deletes a role from the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to delete from the store.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>A <see cref="Task{TResult}" /> that represents the <see cref="IdentityResult" /> of the asynchronous query.</returns>
		public virtual async Task<IdentityResult> DeleteAsync(TRole role,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));

			var result = await Task.Run(() =>
				_factory
					.GetContext()
					.GetTable<TRole>()
					.Where(_ => _.Id.Equals(role.Id) && (_.ConcurrencyStamp == role.ConcurrencyStamp))
					.Delete(), cancellationToken);

			return result == 1 ? IdentityResult.Success : IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
		}

		/// <summary>
		///     Gets the ID for a role from the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose ID should be returned.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>A <see cref="Task{TResult}" /> that contains the ID of the role.</returns>
		public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			return Task.FromResult(ConvertIdToString(role.Id));
		}

		/// <summary>
		///     Gets the name of a role from the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose name should be returned.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>A <see cref="Task{TResult}" /> that contains the name of the role.</returns>
		public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			return Task.FromResult(role.Name);
		}

		/// <summary>
		///     Sets the name of a role in the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose name should be set.</param>
		/// <param name="roleName">The name of the role.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
		public Task SetRoleNameAsync(TRole role, string roleName,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			role.Name = roleName;
			return TaskCache.CompletedTask;
		}

		/// <summary>
		///     Finds the role who has the specified ID as an asynchronous operation.
		/// </summary>
		/// <param name="id">The role ID to look for.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>A <see cref="Task{TResult}" /> that result of the look up.</returns>
		public virtual Task<TRole> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			var roleId = ConvertIdFromString(id);
			return Roles.FirstOrDefaultAsync(u => u.Id.Equals(roleId), cancellationToken);
		}

		/// <summary>
		///     Finds the role who has the specified normalized name as an asynchronous operation.
		/// </summary>
		/// <param name="normalizedName">The normalized role name to look for.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>A <see cref="Task{TResult}" /> that result of the look up.</returns>
		public virtual Task<TRole> FindByNameAsync(string normalizedName,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			return Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName, cancellationToken);
		}

		/// <summary>
		///     Get a role's normalized name as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose normalized name should be retrieved.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>A <see cref="Task{TResult}" /> that contains the name of the role.</returns>
		public virtual Task<string> GetNormalizedRoleNameAsync(TRole role,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			return Task.FromResult(role.NormalizedName);
		}

		/// <summary>
		///     Set a role's normalized name as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose normalized name should be set.</param>
		/// <param name="normalizedName">The normalized name to set</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
		public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			role.NormalizedName = normalizedName;
			return TaskCache.CompletedTask;
		}

		/// <summary>
		///     Dispose the stores
		/// </summary>
		public void Dispose()
		{
			_disposed = true;
		}

		/// <summary>
		///     A navigation property for the roles the store contains.
		/// </summary>
		public virtual IQueryable<TRole> Roles => Context.GetTable<TRole>();

		/// <summary>
		///     Get the claims associated with the specified <paramref name="role" /> as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose claims should be retrieved.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>A <see cref="Task{TResult}" /> that contains the claims granted to a role.</returns>
		public async Task<IList<Claim>> GetClaimsAsync(TRole role,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));

			return await Context.GetTable<TRoleClaim>()
				.Where(rc => rc.RoleId.Equals(role.Id))
				.Select(c => c.ToClaim())
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		///     Adds the <paramref name="claim" /> given to the specified <paramref name="role" />.
		/// </summary>
		/// <param name="role">The role to add the claim to.</param>
		/// <param name="claim">The claim to add to the role.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
		public virtual Task AddClaimAsync(TRole role, Claim claim,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			Context.TryInsertAndSetIdentity(CreateRoleClaim(role, claim));

			return Task.FromResult(false);
		}

		/// <summary>
		///     Removes the <paramref name="claim" /> given from the specified <paramref name="role" />.
		/// </summary>
		/// <param name="role">The role to remove the claim from.</param>
		/// <param name="claim">The claim to remove from the role.</param>
		/// <param name="cancellationToken">
		///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
		///     should be canceled.
		/// </param>
		/// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
		public async Task RemoveClaimAsync(TRole role, Claim claim,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			await Task.Run(() =>
					Context.GetTable<TRoleClaim>()
						.Where(rc => rc.RoleId.Equals(role.Id) && (rc.ClaimValue == claim.Value) && (rc.ClaimType == claim.Type))
						.Delete(),
				cancellationToken);
		}

		/// <summary>
		///     Converts the provided <paramref name="id" /> to a strongly typed key object.
		/// </summary>
		/// <param name="id">The id to convert.</param>
		/// <returns>An instance of <typeparamref name="TKey" /> representing the provided <paramref name="id" />.</returns>
		public virtual TKey ConvertIdFromString(string id)
		{
			if (id == null)
				return default(TKey);
			return (TKey) TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
		}

		/// <summary>
		///     Converts the provided <paramref name="id" /> to its string representation.
		/// </summary>
		/// <param name="id">The id to convert.</param>
		/// <returns>An <see cref="string" /> representation of the provided <paramref name="id" />.</returns>
		public virtual string ConvertIdToString(TKey id)
		{
			if (id.Equals(default(TKey)))
				return null;
			return id.ToString();
		}

		/// <summary>
		///     Throws if this class has been disposed.
		/// </summary>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException(GetType().Name);
		}


		/// <summary>
		///     Creates a entity representing a role claim.
		/// </summary>
		/// <param name="role">The associated role.</param>
		/// <param name="claim">The associated claim.</param>
		/// <returns>The role claim entity.</returns>
		protected abstract TRoleClaim CreateRoleClaim(TRole role, Claim claim);
	}
}